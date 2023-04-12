using Larp.Common;
using Larp.Common.Exceptions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Larp.Data.Mongo.Services;

public class AttachmentManager
{
    private readonly LarpContext _larpContext;
    private readonly ILogger<AttachmentManager> _logger;

    public AttachmentManager(LarpContext larpContext,  ILogger<AttachmentManager> logger)
    {
        _larpContext = larpContext;
        _logger = logger;
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
                MediaType = x.MediaType
            }).ToListAsync())
        .ToArray();

    public async Task<StringResult> Attach(string accountId, Stream data, string fileName, string mediaType, string uploaderAccountId)
    {
        var bytes = new byte[data.Length];
        var _ = await data.ReadAsync(bytes);
        var id = ObjectId.GenerateNewId().ToString();
        await _larpContext.AccountAttachments.InsertOneAsync(new AccountAttachment()
        {
            AttachmentId = id,
            AccountId = accountId,
            Data = bytes,
            MediaType = mediaType,
            FileName = fileName,
            UploadedBy = uploaderAccountId,
            UploadedOn = DateTimeOffset.Now,
            Title = "Untitled"
        });
        return StringResult.Success(id);
    }

}