using Larp.Common;

namespace Larp.Data.Mongo.Services;

public class AttachmentManager
{
    private readonly LarpContext _larpContext;
    private readonly ILogger<AttachmentManager> _logger;
    private readonly IImageModifier _imageModifier;

    public AttachmentManager(LarpContext larpContext, ILogger<AttachmentManager> logger, IImageModifier imageModifier)
    {
        _larpContext = larpContext;
        _logger = logger;
        _imageModifier = imageModifier;
    }

    public async Task SaveAttachment(string attachmentId, AccountAttachment attachment)
    {
        await _larpContext.AccountAttachments.UpdateOneAsync(x => x.AttachmentId == attachmentId,
            Builders<AccountAttachment>.Update
                .Set(x => x.Title, attachment.Title));
    }

    public async Task<AccountAttachment> GetAttachment(string attachmentId) =>
        await _larpContext.AccountAttachments
            .Find(x => x.AttachmentId == attachmentId).FirstOrDefaultAsync()
        ?? throw new ResourceNotFoundException();

    public async Task DeleteAttachment(string attachmentId)
    {
        await _larpContext.AccountAttachments
            .DeleteOneAsync(x => x.AttachmentId == attachmentId);
    }

    public async Task<AccountAttachment[]> GetAccountAttachments(string accountId) =>
        (await _larpContext.AccountAttachments
            .Find(x => x.AccountId == accountId)
            .Project(x => new AccountAttachment()
            {
                AttachmentId = x.AttachmentId,
                UploadedOn = x.UploadedOn,
                UploadedBy = x.UploadedBy,
                Title = x.Title,
                FileName = x.FileName,
                MediaType = x.MediaType,
                ThumbnailFileName = x.ThumbnailFileName,
                ThumbnailMediaType = x.ThumbnailMediaType
            }).ToListAsync())
        .ToArray();

    public async Task<StringResult> Attach(string accountId, Stream data, string fileName, string mediaType,
        string uploaderAccountId)
    {
        var bytes = new byte[data.Length];
        var _ = await data.ReadAsync(bytes);
        var id = ObjectId.GenerateNewId().ToString();
        var attachment = new AccountAttachment()
        {
            AttachmentId = id,
            AccountId = accountId,
            Data = bytes,
            MediaType = mediaType,
            FileName = fileName,
            UploadedBy = uploaderAccountId,
            UploadedOn = DateTimeOffset.Now,
            Title = "Untitled"
        };

        try
        {
            await GenerateThumbnail(attachment);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create thumbnail for attachment {AttachmentId}", attachment.AttachmentId);
        }

        await _larpContext.AccountAttachments.InsertOneAsync(attachment);
        var attachmentCount = await _larpContext.AccountAttachments
            .CountDocumentsAsync(x => x.AccountId == accountId);
        await _larpContext.Accounts.UpdateOneAsync(
            account => account.AccountId == accountId,
            Builders<Account>.Update.Set(account => account.AttachmentCount, attachmentCount));
        return StringResult.Success(id);
    }

    private async Task GenerateThumbnail(AccountAttachment attachment)
    {
        if (attachment.Data == null) return;

        var image = await _imageModifier.GenerateWebp(attachment.Data);
        if (image.Data.Length < attachment.Data.Length)
        {
            attachment.Data = image.Data;
            attachment.MediaType = image.ContentType;
            attachment.FileName = Path.ChangeExtension(attachment.FileName, "webp");
        }

        var thumbnail = await _imageModifier.GenerateWebpThumbnail(512, 512, attachment.Data);
        attachment.ThumbnailData = thumbnail.Data;
        attachment.ThumbnailMediaType = thumbnail.ContentType;
        attachment.ThumbnailFileName = Path.ChangeExtension(attachment.FileName, "thumbnail.webp");
    }
}