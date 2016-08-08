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

		[Fact]
		public async void GetBalanceOk()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57a8793d110000b7161d453a";

			var balance = await _client.GetBalanceAsync();

			Assert.NotNull(balance);
			Assert.Null(balance.Error);
			Assert.Null(balance.ErrorMessage);
			Assert.NotNull(balance.BalanceValue);
			Assert.Equal("success", balance.Status);
		}

		[Fact]
		public async void GetBalance404()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57a8b47a1100007d1c1d4595";

			var balance = await _client.GetBalanceAsync();

			Assert.NotNull(balance);
			Assert.Null(balance.Error);
			Assert.NotNull(balance.ErrorMessage);
			Assert.Null(balance.BalanceValue);
			Assert.Equal("error", balance.Status);
		}
	}
}
