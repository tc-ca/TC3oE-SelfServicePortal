using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace OurTeams.Api;
public class TeamsClient
{
	private readonly HttpClient _client = new HttpClient();
	private readonly string _url;

	public TeamsClient(string webhookUrl)
	{
		_url = webhookUrl;
	}

	public async Task SendMessage(string message)
	{
		dynamic x = new {
			text = message
		};
		var payload = JsonSerializer.Serialize(x);
		var encodedPayload = new StringContent(payload, Encoding.UTF8, "application/json");
		var response = await _client.PostAsync(_url, encodedPayload);
		response.EnsureSuccessStatusCode();
	}
}
