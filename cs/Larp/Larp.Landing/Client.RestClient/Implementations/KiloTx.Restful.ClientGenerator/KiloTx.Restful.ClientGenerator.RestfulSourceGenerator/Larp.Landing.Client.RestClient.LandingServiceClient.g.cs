#nullable enable

using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.FileProviders;
using Larp.Landing.Shared;
using Larp.Data;
using KiloTx.Restful;

namespace Larp.Landing.Client.RestClient;

partial class LandingServiceClient : ILandingService
{
	private HttpClient _httpClient;

	public LandingServiceClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
		WriteIndented = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	async Task<Result> ILandingService.Login(string email, string deviceName)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/auth/login");
		var httpContent = new { email, deviceName };
		httpMessage.Content = JsonContent.Create(httpContent);
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Result>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Result");
	}
	async Task<StringResult> ILandingService.Confirm(string email, string token, string deviceName)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/auth/confirm");
		var httpContent = new { email, token, deviceName };
		httpMessage.Content = JsonContent.Create(httpContent);
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<StringResult>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to StringResult");
	}
	async Task<Result> ILandingService.Logout()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/auth/logout");
		httpMessage.Content = new StringContent("");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Result>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Result");
	}
	async Task<Result> ILandingService.Validate()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/auth/validate");
		httpMessage.Content = new StringContent("");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Result>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Result");
	}
	async Task<Game[]> ILandingService.GetGames()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/larp/games");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Game[]>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Game[]");
	}
	async Task<CharacterSummary[]> ILandingService.GetCharacters()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/larp/characters");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterSummary[]>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to CharacterSummary[]");
	}
	async Task<Account> ILandingService.GetAccount()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/account");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Account>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Account");
	}
	async Task ILandingService.AccountEmailAdd(string email)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/account/email");
		var httpContent = new { email };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task ILandingService.AccountEmailRemove(string email)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/account/email");
		var httpContent = new { email };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task ILandingService.AccountEmailPreferred(string email)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/account/email/preferred");
		var httpContent = new { email };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task ILandingService.AccountUpdate(string? fullName, string? location, string? phone, string? allergies, DateOnly? birthDate)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/account");
		var httpContent = new { fullName, location, phone, allergies, birthDate };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task<EventsAndLetters> ILandingService.GetEvents(EventList list)
	{
		var messageUrl = $"/api/events?list={Uri.EscapeDataString(list.ToString())}";
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, messageUrl);
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<EventsAndLetters>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to EventsAndLetters");
	}
	async Task<Dictionary<string, string>> ILandingService.GetCharacterNames()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/larp/characters/names");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Dictionary<string, string>");
	}
	async Task<EventAttendance[]> ILandingService.GetAttendance()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/events/attendance");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<EventAttendance[]>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to EventAttendance[]");
	}
	async Task<Letter> ILandingService.DraftLetter(string eventId, string letterName)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/letters/new");
		var httpContent = new { eventId, letterName };
		httpMessage.Content = JsonContent.Create(httpContent);
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Letter>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Letter");
	}
	async Task<Letter> ILandingService.GetLetter(string letterId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/letters/{letterId}");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Letter>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to Letter");
	}
	async Task ILandingService.SaveLetter(string letterId, Letter letter)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/letters/{letterId}");
		var httpContent = new { letter };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task<EventsAndLetters> ILandingService.GetEventLetter(string eventId, string letterName)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/letters/events/{eventId}/{letterName}");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<EventsAndLetters>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to EventsAndLetters");
	}
	async Task<IFileInfo> ILandingService.GetAttachment(string attachmentId, string fileName)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/attachments/{attachmentId}/{fileName}");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<IFileInfo>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to IFileInfo");
	}
}
