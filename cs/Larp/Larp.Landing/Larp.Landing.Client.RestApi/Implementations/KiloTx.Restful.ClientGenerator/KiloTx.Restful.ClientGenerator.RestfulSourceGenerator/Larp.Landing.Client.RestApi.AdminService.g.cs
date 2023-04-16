#nullable enable

using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;
using System.IO;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.FileProviders;
using Larp.Landing.Shared;
using Larp.Data.MwFifth;
using Larp.Data;
using KiloTx.Restful;


namespace Larp.Landing.Client.RestApi;

partial class AdminService : IAdminService
{
	async Task<Dashboard> IAdminService.GetDashboard()
	{
		var response = await _httpClient.PostAsync("/api/blah", new StringContent(""));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Dashboard>(stream) ?? throw new BadRequestException();
	}
	async Task<Account[]> IAdminService.GetAccounts()
	{
		var response = await _httpClient.PostAsync("/api/blah", new StringContent(""));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Account[]>(stream) ?? throw new BadRequestException();
	}
	async Task<Account> IAdminService.GetAccount(string accountId)
	{
		var httpContent = new { accountId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Account>(stream) ?? throw new BadRequestException();
	}
	async Task<CharacterSummary[]> IAdminService.GetAccountCharacters(string accountId)
	{
		var httpContent = new { accountId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterSummary[]>(stream) ?? throw new BadRequestException();
	}
	async Task IAdminService.UpdateAccount(string accountId, string? name, string? location, string? phone, DateOnly? birthDate, string? notes, int? discount)
	{
		var httpContent = new { accountId, name, location, phone, birthDate, notes, discount };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task IAdminService.AddAccountRole(string accountId, AccountRole role)
	{
		var httpContent = new { accountId, role };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task IAdminService.RemoveAccountRole(string accountId, AccountRole role)
	{
		var httpContent = new { accountId, role };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task<StringResult> IAdminService.AddAdminAccount(string fullName, string emailAddress)
	{
		var httpContent = new { fullName, emailAddress };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<StringResult>(stream) ?? throw new BadRequestException();
	}
	async Task<CharacterAccountSummary[]> IAdminService.GetMwFifthCharacters(CharacterState state)
	{
		var httpContent = new { state };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterAccountSummary[]>(stream) ?? throw new BadRequestException();
	}
	async Task<CharacterAndRevision> IAdminService.GetMwFifthCharacter(string characterId)
	{
		var httpContent = new { characterId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterAndRevision>(stream) ?? throw new BadRequestException();
	}
	async Task<CharacterAndRevision> IAdminService.GetMwFifthCharacterLatest(string characterId)
	{
		var httpContent = new { characterId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterAndRevision>(stream) ?? throw new BadRequestException();
	}
	async Task<CharacterAndRevisions> IAdminService.GetMwFifthCharacterRevisions(string characterId)
	{
		var httpContent = new { characterId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterAndRevisions>(stream) ?? throw new BadRequestException();
	}
	async Task IAdminService.ApproveMwFifthCharacter(string characterId)
	{
		var httpContent = new { characterId };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task IAdminService.RejectMwFifthCharacter(string characterId)
	{
		var httpContent = new { characterId };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task<CharacterAndRevision> IAdminService.ReviseMwFifthCharacter(string characterId)
	{
		var httpContent = new { characterId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterAndRevision>(stream) ?? throw new BadRequestException();
	}
	async Task IAdminService.SaveMwFifthCharacter(CharacterRevision revision)
	{
		var httpContent = new { revision };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task IAdminService.DeleteMwFifthCharacter(string characterId)
	{
		var httpContent = new { characterId };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task IAdminService.MoveMwFifthCharacter(string characterId, string newAccountId)
	{
		var httpContent = new { characterId, newAccountId };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task<EventAndLetters[]> IAdminService.GetEvents()
	{
		var response = await _httpClient.PostAsync("/api/blah", new StringContent(""));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<EventAndLetters[]>(stream) ?? throw new BadRequestException();
	}
	async Task<Event> IAdminService.GetEvent(string eventId)
	{
		var httpContent = new { eventId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Event>(stream) ?? throw new BadRequestException();
	}
	async Task IAdminService.SaveEvent(string eventId, Event @event)
	{
		var httpContent = new { eventId, @event };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task IAdminService.DeleteEvent(string eventId)
	{
		var httpContent = new { eventId };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task IAdminService.SetEventAttendance(string eventId, string accountId, bool attended, int? moonstone, string[] characterIds)
	{
		var httpContent = new { eventId, accountId, attended, moonstone, characterIds };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task<Dictionary<string, AccountName>> IAdminService.GetAccountNames()
	{
		var response = await _httpClient.PostAsync("/api/blah", new StringContent(""));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Dictionary<string, AccountName>>(stream) ?? throw new BadRequestException();
	}
	async Task<Attendance[]> IAdminService.GetEventAttendances(string eventId)
	{
		var httpContent = new { eventId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Attendance[]>(stream) ?? throw new BadRequestException();
	}
	async Task<StringResult> IAdminService.Import(Stream data)
	{
		var httpContent = new { data };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<StringResult>(stream) ?? throw new BadRequestException();
	}
	async Task<IFileInfo> IAdminService.Export()
	{
		var response = await _httpClient.PostAsync("/api/blah", new StringContent(""));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<IFileInfo>(stream) ?? throw new BadRequestException();
	}
	async Task<IFileInfo> IAdminService.ExportLetters(string eventId)
	{
		var httpContent = new { eventId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<IFileInfo>(stream) ?? throw new BadRequestException();
	}
	async Task IAdminService.MergeAccounts(string fromAccountId, string toAccountId)
	{
		var httpContent = new { fromAccountId, toAccountId };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task IAdminService.AddAccountEmail(string accountId, string email)
	{
		var httpContent = new { accountId, email };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task IAdminService.RemoveAccountEmail(string accountId, string email)
	{
		var httpContent = new { accountId, email };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task<LetterTemplate> IAdminService.DraftLetterTemplate()
	{
		var response = await _httpClient.PostAsync("/api/blah", new StringContent(""));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<LetterTemplate>(stream) ?? throw new BadRequestException();
	}
	async Task IAdminService.SaveLetterTemplate(string templateId, LetterTemplate template)
	{
		var httpContent = new { templateId, template };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task<LetterTemplate[]> IAdminService.GetLetterTemplates()
	{
		var response = await _httpClient.PostAsync("/api/blah", new StringContent(""));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<LetterTemplate[]>(stream) ?? throw new BadRequestException();
	}
	async Task<LetterTemplate[]> IAdminService.GetLetterTemplateNames()
	{
		var response = await _httpClient.PostAsync("/api/blah", new StringContent(""));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<LetterTemplate[]>(stream) ?? throw new BadRequestException();
	}
	async Task<LetterTemplate> IAdminService.GetLetterTemplate(string templateId)
	{
		var httpContent = new { templateId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<LetterTemplate>(stream) ?? throw new BadRequestException();
	}
	async Task IAdminService.ApproveLetter(string letterId)
	{
		var httpContent = new { letterId };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task IAdminService.RejectLetter(string letterId)
	{
		var httpContent = new { letterId };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task<Letter[]> IAdminService.GetSubmittedLetters()
	{
		var response = await _httpClient.PostAsync("/api/blah", new StringContent(""));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Letter[]>(stream) ?? throw new BadRequestException();
	}
	async Task<EventsAndLetters> IAdminService.GetEventLetters(string eventId)
	{
		var httpContent = new { eventId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<EventsAndLetters>(stream) ?? throw new BadRequestException();
	}
	async Task<Letter[]> IAdminService.GetTemplateLetters(string templateId)
	{
		var httpContent = new { templateId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Letter[]>(stream) ?? throw new BadRequestException();
	}
	async Task<AccountAttachment[]> IAdminService.GetAccountAttachments(string accountId)
	{
		var httpContent = new { accountId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<AccountAttachment[]>(stream) ?? throw new BadRequestException();
	}
	async Task<StringResult> IAdminService.Attach(string accountId, Stream data, string fileName, string mediaType)
	{
		var httpContent = new { accountId, data, fileName, mediaType };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<StringResult>(stream) ?? throw new BadRequestException();
	}
	async Task IAdminService.SaveAttachment(string attachmentId, AccountAttachment attachment)
	{
		var httpContent = new { attachmentId, attachment };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task<AccountAttachment> IAdminService.GetAttachment(string attachmentId)
	{
		var httpContent = new { attachmentId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<AccountAttachment>(stream) ?? throw new BadRequestException();
	}
	async Task IAdminService.DeleteAttachment(string attachmentId)
	{
		var httpContent = new { attachmentId };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task<Event> IAdminService.DraftEvent()
	{
		var response = await _httpClient.PostAsync("/api/blah", new StringContent(""));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Event>(stream) ?? throw new BadRequestException();
	}
	async Task IAdminService.SetMwFifthCharacterNotes(string characterId, string? notes)
	{
		var httpContent = new { characterId, notes };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task IAdminService.SetAccountNotes(string accountId, string? notes)
	{
		var httpContent = new { accountId, notes };
		await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
	}
	async Task<PreregistrationNotes> IAdminService.GetEventNotes(string eventId)
	{
		var httpContent = new { eventId };
		var response = await _httpClient.PostAsync("/api/blah", JsonContent.Create(httpContent));
		await using var stream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<PreregistrationNotes>(stream) ?? throw new BadRequestException();
	}
}
