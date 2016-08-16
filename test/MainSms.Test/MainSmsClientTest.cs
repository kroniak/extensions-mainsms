using System;
using System.Collections.Generic;
using System.Linq;
using MainSMS;
using Xunit;
// ReSharper disable All

namespace MainSms.Test
{
	public class MainSmsClientTest
	{
		private readonly MainSmsClient _client = new MainSmsClient("test", "test").GetTestClient();

		[Fact]
		public async void CancelAsync()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57aa67ec1200008402739c9e";

			var info = await _client.CancelAsync(new List<int> { 91911719 });

			Assert.NotNull(info);
			Assert.NotEmpty(info.Statuses);
			Assert.Null(info.ErrorCode);
			Assert.Null(info.ErrorMessage);
			Assert.Equal("success", info.Status);
		}

		[Fact]
		public async void GetBalance()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57a8793d110000b7161d453a";

			var balance = await _client.GetBalanceAsync();

			Assert.NotNull(balance);
			Assert.NotEmpty(balance.Url);
			Assert.Null(balance.ErrorCode);
			Assert.Null(balance.ErrorMessage);
			Assert.NotNull(balance.Url);
			Assert.NotNull(balance.Balance);
			Assert.Equal("success", balance.Status);
		}

		[Fact]
		public async void GetBalance404()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57a8b47a1100007d1c1d4595";

			var balance = await _client.GetBalanceAsync();

			Assert.NotNull(balance);
			Assert.Null(balance.ErrorCode);
			Assert.NotNull(balance.ErrorMessage);
			Assert.NotNull(balance.Url);
			Assert.Equal(0, balance.Balance);
			Assert.Equal("error", balance.Status);
		}

		[Fact]
		public async void GetBalanceError()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57a7ebc9110000500a1d4463";

			var balance = await _client.GetBalanceAsync();

			Assert.NotNull(balance);
			Assert.NotNull(balance.ErrorCode);
			Assert.NotNull(balance.ErrorMessage);
			Assert.NotNull(balance.Url);
			Assert.Equal(0, balance.Balance);
			Assert.Equal("error", balance.Status);
		}

		[Fact]
		public async void GetInfoAsync()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57a8ce31110000181f1d45ac";
			var infos = await _client.GetInfoAsync(new List<string> { "79210000000", "79210000001" });

			Assert.NotNull(infos);
			Assert.NotEmpty(infos.Phones);
			Assert.Null(infos.ErrorCode);
			Assert.Null(infos.ErrorMessage);
			Assert.NotNull(infos.Url);
			Assert.Equal("success", infos.Status);
		}

		[Fact]
		public async void GetPriceAsync()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57a903861100005e011659ef";

			var price = await _client.GetPriceAsync(new List<string> { "79210000000", "79210000001" }, "Test message!");

			Assert.NotNull(price);
			Assert.NotEmpty(price.Recipients);
			Assert.NotNull(price.Balance);
			Assert.NotNull(price.MessageCount);
			Assert.NotNull(price.PartsCount);
			Assert.NotNull(price.Price);
			Assert.Null(price.ErrorCode);
			Assert.Null(price.ErrorMessage);
			Assert.Equal("success", price.Status);
		}

		[Fact]
		public async void GetStatusesAsync()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57aa67ec1200008402739c9e";

			var info = await _client.GetStatusesAsync(new List<int> { 91911719 });

			Assert.NotNull(info);
			Assert.NotEmpty(info.Statuses);
			Assert.Null(info.ErrorCode);
			Assert.Null(info.ErrorMessage);
			Assert.Equal("success", info.Status);
		}

		[Fact]
		public async void SendAsync()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57aa5d201200003501739c99";

			var result = await _client.SendAsync(
				new List<string> { "70000000000", "70000000001" },
				"test message",
				testMode: true);

			Assert.NotNull(result);
			Assert.NotEmpty(result.Url);
			Assert.NotEmpty(result.Recipients);
			Assert.NotEmpty(result.MessageIds);
			Assert.NotNull(result.Balance);
			Assert.NotNull(result.MessageCount);
			Assert.NotNull(result.PartsCount);
			Assert.NotNull(result.Price);
			Assert.Null(result.ErrorCode);
			Assert.Null(result.ErrorMessage);
			Assert.Equal(true, result.TestMode);
			Assert.Equal("success", result.Status);
		}

		[Fact]
		public async void SendBatchAsync()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57b33fbd1000008816b478ab";

			var messages = new Dictionary<string, string>();
			messages.Add("87000000001", "test message 1");
			messages.Add("87000000002", "test message 2");

			var result = await _client.SendBatchAsync(messages, sender: "sendertest", testMode: true);

			Assert.NotNull(result);
			Assert.NotNull(result.PartsCount);
			Assert.NotNull(result.Price);
			Assert.NotNull(result.RecipientsCount);
			Assert.Null(result.ErrorCode);
			Assert.Null(result.ErrorMessage);
			Assert.Equal("success", result.Status);
		}

		[Fact]
		public async void SendBatchErrorAsync()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57b340241000009116b478ad";

			var messages = new Dictionary<string, string>();
			messages.Add("89214045559", "test message 1");
			messages.Add("89213020041", "test message 2");

			var result = await _client.SendBatchAsync(messages, sender: "sendertest", testMode: true);

			Assert.NotNull(result);
			Assert.Equal(0, result.PartsCount);
			Assert.Equal(0, result.Price);
			Assert.Equal(0, result.RecipientsCount);
			Assert.Null(result.ErrorCode);
			Assert.NotNull(result.ErrorMessage);
			Assert.Equal("error", result.Status);
		}

		public async void ToReadme()
		{
			// Create client
			var client = new MainSmsClient("you project name", "your api key");

			// Getting your project balance
			var balanse = await client.GetBalanceAsync();

			Console.WriteLine(balanse.Status == "success" ? $"balance is {balanse.Balance}"
				: $"Error is {balanse.ErrorMessage}");

			// Getting send price information and right format phone numbers
			var price = await client.GetPriceAsync("79214445566", "test messages");

			Console.WriteLine(price.Status == "success" ?
				$"Price is {price.Price}. Total messages count is {price.MessageCount}"
				: $"Error is {price.ErrorMessage}");

			// Sending messages
			var sendResult = await client.SendAsync("79214445566,+89214445566", "test message");

			Console.WriteLine(sendResult.Status == "success" ?
				$"Price is {sendResult.Price}. Total messages count is {sendResult.MessageCount}"
				: $"Error is {sendResult.ErrorMessage}");

			if (sendResult.MessageIds.Any()) //print receaved message ids.
				sendResult.MessageIds.ToList().ForEach(id => Console.WriteLine($"Message id is {id}"));

			// Batch sending messages
			var messages = new Dictionary<string, string>();
			messages.Add("89214045559", "test message 1");
			messages.Add("89213020041", "test message 2");

			var result = await _client.SendBatchAsync(messages, sender: "sendertest");

			// If you want only test without real sending the messages 
			// pass testMode param, MessageIds will be empty
			var testSendResult = await client.SendAsync("79214445566,+89214445566",
				"test messages", testMode: true);

			// Getting delivery statuses of the messages
			var statuses = await client.GetStatusesAsync(sendResult.MessageIds);

			if (statuses.Status == "success") // print message statuses
				if (statuses.Statuses.Any())
					foreach (var status in statuses.Statuses)
						Console.WriteLine(
							$"The status of the message {status.Key} is {status.Value}");
				else
					Console.WriteLine($"Error is {statuses.ErrorMessage}");

			// Send a delayed message
			var delayedSendResult = await client.SendAsync("79214445566,+89214445566",
						"test messages", startDateTime: new DateTime(2016, 08, 12));

			// Try to cancel these messages
			if (delayedSendResult.Status == "success" && delayedSendResult.MessageIds.Any())
			{
				var cancelResult = await client.CancelAsync(delayedSendResult.MessageIds);
				if (cancelResult.Status == "success") // print messages statuses
					if (cancelResult.Statuses.Any())
						foreach (var status in cancelResult.Statuses)
							Console.WriteLine($"The status of the message {status.Key} is {status.Value}");
			}
		}
	}
}