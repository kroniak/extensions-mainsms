using MainSMS;
using Xunit;

namespace MainSms.Test
{
	public class MainSmsClientTest
	{
		private readonly MainSmsClient _client = new MainSmsClient("test", "test") { TestMode=true };

		[Fact]
		public async void GetBalanceError()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57a7ebc9110000500a1d4463";

			var balance = await _client.GetBalanceAsync();

			Assert.NotNull(balance);
			Assert.NotNull(balance.Error);
			Assert.NotNull(balance.ErrorMessage);
			Assert.Null(balance.BalanceValue);
			Assert.Equal("error", balance.Status);
		}
	}
}
