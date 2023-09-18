using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OurAzure.Api.Services;

public abstract class RestClient
{
	protected readonly HttpClient Client = new HttpClient();
	public RestClient(Azure.Core.TokenCredential cred) : this(cred.GetAuthHeader("https://management.azure.com/.default"))
	{
	}

	public RestClient(AuthenticationHeaderValue auth)
	{
		Client.DefaultRequestHeaders.Authorization = auth;
		Client.DefaultRequestHeaders.Accept.Clear();
		Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
	}

	protected async Task<T> RequestAsync<T>(string url, object payload, Func<string, StringContent, Task<HttpResponseMessage>> method)
	{
		var jsonPayload = JsonSerializer.Serialize(payload);
		var encodedPayload = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
		var resp = await method.Invoke(url, encodedPayload);
		var body = await resp.Content.ReadAsStringAsync();
		if (!resp.IsSuccessStatusCode) throw new System.Exception(body);
		var rtn = JsonSerializer.Deserialize<T>(body)!;
		return rtn;
	}

	protected async Task<T> PostAsync<T>(string url, object payload)
	{
		return await RequestAsync<T>(url, payload, Client.PostAsync);
	}
	protected async Task<T> PutAsync<T>(string url, object payload)
	{
		return await RequestAsync<T>(url, payload, Client.PutAsync);
	}

	protected async Task<T> PatchAsync<T>(string url, object payload)
	{
		return await RequestAsync<T>(url, payload, Client.PatchAsync);
	}

	protected async Task<T> GetAsync<T>(string url)
	{
		var resp = await Client.GetAsync(url);
		var body = await resp.Content.ReadAsStringAsync();
		if (!resp.IsSuccessStatusCode) throw new System.Exception(body);
		return JsonSerializer.Deserialize<T>(body)!;
	}
}