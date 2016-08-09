﻿using System.Collections.Generic;
using MainSMS;
using Xunit;

namespace MainSms.Test
{
	public class MainSmsClientTest
	{
		private readonly MainSmsClient _client = new MainSmsClient("test", "test").GetTestClient();

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
		public async void GetBalance()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57a8793d110000b7161d453a";

			var balance = await _client.GetBalanceAsync();

			Assert.NotNull(balance);
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
		public async void GetPriceAsync()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57a903861100005e011659ef";

			var price = await _client.GetPriceAsync(new List<string> { "79210000000", "79210000001" }, "Hello!");

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
		public async void SendAsync()
		{
			_client.TestUrl = "http://www.mocky.io/v2/57aa5d201200003501739c99";

			var result = await _client.SendAsync(
				recipients: new List<string> { "70000000000", "70000000001" },
				message: "test",
				testMode: true);

			Assert.NotNull(result);
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
	}
}
