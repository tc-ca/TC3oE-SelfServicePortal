using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;


namespace SelfServicePortal.Test;

public class TeamsTests : BaseTest
{
	public TeamsTests(ITestOutputHelper helper) : base(helper) {}
	
	[Fact]
	public async Task SendMessage()
	{
		await Services.TeamsClient.SendMessage("Hello from the test suite!");
		// Invoke-RestMethod -Method post -ContentType 'Application/Json' -Body '{"text":"Hello World!"}' -Uri <YOUR WEBHOOK URL>
	}
}