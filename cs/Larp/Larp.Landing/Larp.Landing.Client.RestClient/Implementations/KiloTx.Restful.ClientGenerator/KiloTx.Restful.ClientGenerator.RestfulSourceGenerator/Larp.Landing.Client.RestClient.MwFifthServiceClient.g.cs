#nullable enable

using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;
using System;
using Larp.Landing.Shared.MwFifth;
using Larp.Data.MwFifth;
using KiloTx.Restful;

namespace Larp.Landing.Client.RestClient;

partial class MwFifthServiceClient : IMwFifthService
{
	private HttpClient _httpClient;

	public MwFifthServiceClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
		WriteIndented = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	async Task<GameState> IMwFifthService.GetGameState(string lastRevision)
	{
		var messageUrl = $"api/mw5e/gameState?lastRevision={Uri.EscapeDataString(lastRevision.ToString())}";
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, messageUrl);
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<GameState>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to GameState");
	}
	async Task<CharacterAndRevision> IMwFifthService.GetCharacter(string characterId)
	{
		var messageUrl = $"api/mw5e/character?characterId={Uri.EscapeDataString(characterId.ToString())}";
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, messageUrl);
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterAndRevision>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to CharacterAndRevision");
	}
	async Task<CharacterAndRevision> IMwFifthService.ReviseCharacter(string characterId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"api/mw5e/character/revise");
		var httpContent = new { characterId };
		httpMessage.Content = JsonContent.Create(httpContent);
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterAndRevision>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to CharacterAndRevision");
	}
	async Task<CharacterAndRevision> IMwFifthService.GetNewCharacter()
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"api/mw5e/character/new");
		var httpResponse = await _httpClient.SendAsync(httpMessage);
		await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<CharacterAndRevision>(httpResponseStream, JsonOptions)
			?? throw new BadRequestException("Unable to deserialize to CharacterAndRevision");
	}
	async Task IMwFifthService.SaveCharacter(CharacterRevision revision)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Post, $"api/mw5e/character");
		var httpContent = new { revision };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
	async Task IMwFifthService.DeleteCharacter(string characterId)
	{
		var httpMessage = new HttpRequestMessage(HttpMethod.Delete, $"api/mw5e/character");
		var httpContent = new { characterId };
		httpMessage.Content = JsonContent.Create(httpContent);
		await _httpClient.SendAsync(httpMessage);
	}
}
