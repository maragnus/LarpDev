#nullable enable

using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.IO;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.FileProviders;
using Larp.Landing.Shared;
using Larp.Data.MwFifth;
using Larp.Data;
using KiloTx.Restful;

namespace Larp.Landing.Client.RestClient;

partial class AdminServiceClient : IAdminService
{
	private HttpClient _httpClient;

	public AdminServiceClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
		WriteIndented = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	async Task<Dashboard> IAdminService.GetDashboard()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/dashboard");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Dashboard>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Dashboard");
	}
	async Task<Account[]> IAdminService.GetAccounts()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/accounts");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Account[]>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Account[]");
	}
	async Task<Account> IAdminService.GetAccount(string accountId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/accounts/{accountId}");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Account>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Account");
	}
	async Task<CharacterSummary[]> IAdminService.GetAccountCharacters(string accountId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/accounts/{accountId}/characters");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterSummary[]>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to CharacterSummary[]");
	}
	async Task IAdminService.UpdateAccount(string accountId, string? name, string? location, string? phone, DateOnly? birthDate, string? notes, int? discount)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/accounts/{accountId}");
		var httpContent = new { name, location, phone, birthDate, notes, discount };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task IAdminService.AddAccountRole(string accountId, AccountRole role)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/accounts/{accountId}/roles/{role}");
		httpMessage.Content = new StringContent("");
		await _httpClient.SendAsync(httpMessage);
	}
	async Task IAdminService.RemoveAccountRole(string accountId, AccountRole role)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/admin/accounts/{accountId}/roles/{role}");
		httpMessage.Content = new StringContent("");
		await _httpClient.SendAsync(httpMessage);
	}
	async Task<StringResult> IAdminService.AddAdminAccount(string fullName, string emailAddress)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/accounts/admin");
		var httpContent = new { fullName, emailAddress };
		httpMessage.Content = JsonContent.Create(httpContent);
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<StringResult>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to StringResult");
	}
	async Task<CharacterAccountSummary[]> IAdminService.GetMwFifthCharacters(CharacterState state)
	{
		var messageUrl = $"/api/admin/mw5e/characters?state={Uri.EscapeDataString(state.ToString())}";
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, messageUrl);
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterAccountSummary[]>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to CharacterAccountSummary[]");
	}
	async Task<CharacterAndRevision> IAdminService.GetMwFifthCharacter(string characterId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/mw5e/characters/{characterId}");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterAndRevision>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to CharacterAndRevision");
	}
	async Task<CharacterAndRevision> IAdminService.GetMwFifthCharacterLatest(string characterId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/mw5e/characters/{characterId}/latest");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterAndRevision>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to CharacterAndRevision");
	}
	async Task<CharacterAndRevisions> IAdminService.GetMwFifthCharacterRevisions(string characterId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/mw5e/characters/{characterId}/revisions");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterAndRevisions>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to CharacterAndRevisions");
	}
	async Task IAdminService.ApproveMwFifthCharacter(string characterId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/mw5e/characters/{characterId}/approve");
		httpMessage.Content = new StringContent("");
		await _httpClient.SendAsync(httpMessage);
	}
	async Task IAdminService.RejectMwFifthCharacter(string characterId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/mw5e/characters/{characterId}/reject");
		httpMessage.Content = new StringContent("");
		await _httpClient.SendAsync(httpMessage);
	}
	async Task<CharacterAndRevision> IAdminService.ReviseMwFifthCharacter(string characterId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/mw5e/characters/{characterId}/revise");
		httpMessage.Content = new StringContent("");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterAndRevision>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to CharacterAndRevision");
	}
	async Task IAdminService.SaveMwFifthCharacter(CharacterRevision revision)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/mw5e/characters");
		var httpContent = new { revision };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task IAdminService.DeleteMwFifthCharacter(string characterId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/admin/mw5e/characters/{characterId}");
		httpMessage.Content = new StringContent("");
		await _httpClient.SendAsync(httpMessage);
	}
	async Task IAdminService.MoveMwFifthCharacter(string characterId, string newAccountId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/mw5e/characters/{characterId}/move");
		var httpContent = new { newAccountId };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task<EventAndLetters[]> IAdminService.GetEvents()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/events");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<EventAndLetters[]>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to EventAndLetters[]");
	}
	async Task<Event> IAdminService.GetEvent(string eventId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/events/{eventId}");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Event>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Event");
	}
	async Task IAdminService.SaveEvent(string eventId, Event @event)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/events/{eventId}");
		var httpContent = new { @event };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task IAdminService.DeleteEvent(string eventId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/admin/events/{eventId}");
		httpMessage.Content = new StringContent("");
		await _httpClient.SendAsync(httpMessage);
	}
	async Task IAdminService.SetEventAttendance(string eventId, string accountId, bool attended, int? moonstone, string[] characterIds)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/events/{eventId}/attendance/{accountId}");
		var httpContent = new { attended, moonstone, characterIds };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task<Dictionary<string, AccountName>> IAdminService.GetAccountNames()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/accounts/names");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Dictionary<string, AccountName>>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Dictionary<string, AccountName>");
	}
	async Task<Attendance[]> IAdminService.GetEventAttendances(string eventId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/events/{eventId}/attendance");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Attendance[]>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Attendance[]");
	}
	async Task<StringResult> IAdminService.Import(Stream data)
	{
		var httpStreamContent = new StreamContent(data);
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/data/import")
		{
			Content = new MultipartFormDataContent { { httpStreamContent, "file", "unnamed" } }
		};
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<StringResult>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to StringResult");
	}
	async Task<IFileInfo> IAdminService.Export()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/data/export");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<IFileInfo>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to IFileInfo");
	}
	async Task<IFileInfo> IAdminService.ExportLetters(string eventId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/letters/events/{eventId}/export");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<IFileInfo>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to IFileInfo");
	}
	async Task IAdminService.MergeAccounts(string fromAccountId, string toAccountId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/accounts/merge");
		var httpContent = new { fromAccountId, toAccountId };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task IAdminService.AddAccountEmail(string accountId, string email)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/accounts/{accountId}/emails");
		var httpContent = new { email };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task IAdminService.RemoveAccountEmail(string accountId, string email)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/admin/accounts/{accountId}/emails");
		var httpContent = new { email };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task<LetterTemplate> IAdminService.DraftLetterTemplate()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/letters/templates/new");
		httpMessage.Content = new StringContent("");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<LetterTemplate>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to LetterTemplate");
	}
	async Task IAdminService.SaveLetterTemplate(string templateId, LetterTemplate template)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/letters/templates/{templateId}");
		var httpContent = new { template };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task<LetterTemplate[]> IAdminService.GetLetterTemplates()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/letters/templates");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<LetterTemplate[]>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to LetterTemplate[]");
	}
	async Task<LetterTemplate[]> IAdminService.GetLetterTemplateNames()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/letters/templates/names");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<LetterTemplate[]>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to LetterTemplate[]");
	}
	async Task<LetterTemplate> IAdminService.GetLetterTemplate(string templateId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/letters/templates/{templateId}");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<LetterTemplate>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to LetterTemplate");
	}
	async Task IAdminService.ApproveLetter(string letterId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/letters/{letterId}/approve");
		httpMessage.Content = new StringContent("");
		await _httpClient.SendAsync(httpMessage);
	}
	async Task IAdminService.RejectLetter(string letterId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/letters/{letterId}/reject");
		httpMessage.Content = new StringContent("");
		await _httpClient.SendAsync(httpMessage);
	}
	async Task<Letter[]> IAdminService.GetSubmittedLetters()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/letters/submitted");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Letter[]>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Letter[]");
	}
	async Task<EventsAndLetters> IAdminService.GetEventLetters(string eventId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/letters/events/{eventId}");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<EventsAndLetters>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to EventsAndLetters");
	}
	async Task<Letter[]> IAdminService.GetTemplateLetters(string templateId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/letters/templates/{templateId}/letters");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Letter[]>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Letter[]");
	}
	async Task<AccountAttachment[]> IAdminService.GetAccountAttachments(string accountId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/accounts/{accountId}/attachments");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<AccountAttachment[]>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to AccountAttachment[]");
	}
	async Task<StringResult> IAdminService.Attach(string accountId, Stream data, string fileName, string mediaType)
	{
		var httpStreamContent = new StreamContent(data) { Headers = { ContentType = new MediaTypeHeaderValue(mediaType) }};
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/accounts/{accountId}/attachments/attach")
		{
			Content = new MultipartFormDataContent { { httpStreamContent, "file", fileName } }
		};
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<StringResult>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to StringResult");
	}
	async Task IAdminService.SaveAttachment(string attachmentId, AccountAttachment attachment)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/attachments/{attachmentId}");
		var httpContent = new { attachment };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task<AccountAttachment> IAdminService.GetAttachment(string attachmentId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/attachments/{attachmentId}");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<AccountAttachment>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to AccountAttachment");
	}
	async Task IAdminService.DeleteAttachment(string attachmentId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/admin/attachments/{attachmentId}");
		httpMessage.Content = new StringContent("");
		await _httpClient.SendAsync(httpMessage);
	}
	async Task<Event> IAdminService.DraftEvent()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/events/new");
		httpMessage.Content = new StringContent("");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Event>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Event");
	}
	async Task IAdminService.SetMwFifthCharacterNotes(string characterId, string? notes)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/mw5e/characters/{characterId}/notes");
		var httpContent = new { notes };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task IAdminService.SetAccountNotes(string accountId, string? notes)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/accounts/{accountId}/notes");
		var httpContent = new { notes };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task<PreregistrationNotes> IAdminService.GetEventNotes(string eventId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/events/{eventId}/notes");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<PreregistrationNotes>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to PreregistrationNotes");
	}
}
