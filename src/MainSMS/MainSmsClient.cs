﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Flurl;
using Flurl.Http;
using Flurl.Http.Xml;

namespace MainSMS
{
	/// <summary>
	/// Main client class to MainSMS API.
	/// </summary>
	public class MainSmsClient
	{
		// Default  type for response
		private const string ResponseType = "xml";

		private string _apiUrl;

		// Defalut API URL
		private string ApiUrl
		{
			get
			{
				return _testMode ? TestUrl : _apiUrl;
			}
			set { _apiUrl = value; }
		}

		private readonly string _apiKey;

		/// <summary>
		/// Gets or sets a value indicating whether [test mode].
		/// </summary>
		/// <value>
		///   <c>true</c> if [test mode]; otherwise, <c>false</c>.
		/// </value>
		private bool _testMode;

		/// <summary>
		/// Gets or sets the test URL.
		/// </summary>
		/// <value>
		/// The test URL.
		/// </value>
		public string TestUrl { private get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MainSmsClient"/> class.
		/// </summary>
		/// <param name="project">The project.</param>
		/// <param name="apiKey">The API key.</param>
		public MainSmsClient(string project, string apiKey)
		{
			if (string.IsNullOrEmpty(project))
				throw new ArgumentNullException(nameof(project));
			if (string.IsNullOrEmpty(apiKey))
				throw new ArgumentNullException(nameof(apiKey));

			_apiKey = apiKey;

			ApiUrl = "https://mainsms.ru/api/mainsms/message/"
				.SetQueryParams(new { project, format = ResponseType });
		}

		/// <summary>
		/// Gets the test client.
		/// </summary>
		public MainSmsClient GetTestClient()
		{
			_testMode = true;
			return this;
		}

		/// <summary>
		/// Gets the test client.
		/// </summary>
		/// <param name="testUrl">The test URL.</param>
		public MainSmsClient GetTestClient(string testUrl)
		{
			TestUrl = testUrl;
			return GetTestClient();
		}

		/// <summary>
		/// Gets the balance.
		/// </summary>
		public async Task<BalanceInfo> GetBalanceAsync()
		{
			var url = ApiUrl
				.AppendPathSegment("balance");

			var responce = await GetXDocumentAsync(url);

			return new BalanceInfo(responce);
		}

		/// <summary>
		/// Gets the phones information asynchronous.
		/// </summary>
		/// <param name="phones">The phones separeted by semicolons.</param>
		public async Task<PhonesInfo> GetInfoAsync(string phones)
		{
			var url = ApiUrl
				.AppendPathSegment("info")
				.SetQueryParam("phones", phones);

			var responce = await GetXDocumentAsync(url);

			return new PhonesInfo(responce);
		}

		/// <summary>
		/// Gets the phones information asynchronous.
		/// </summary>
		/// <param name="phones">The phones.</param>
		public async Task<PhonesInfo> GetInfoAsync(IEnumerable<string> phones)
		{
			var phonesSemi = string.Join(",", phones.ToArray());

			return await GetInfoAsync(phonesSemi);
		}

		/// <summary>
		/// Gets the price asynchronous.
		/// </summary>
		/// <param name="recipients">The recipients phones.</param>
		/// <param name="message">The message in UTF8.</param>
		public async Task<PriceInfo> GetPriceAsync(string recipients, string message)
		{
			var url = ApiUrl
				.AppendPathSegment("price")
				.SetQueryParam("recipients", recipients)
				.SetQueryParam("message", message);

			var responce = await GetXDocumentAsync(url);

			return new PriceInfo(responce);
		}

		/// <summary>
		/// Gets the price asynchronous.
		/// </summary>
		/// <param name="recipients">The recipients phones.</param>
		/// <param name="message">The message in UTF8.</param>
		public async Task<PriceInfo> GetPriceAsync(IEnumerable<string> recipients, string message)
		{
			var recipientsSemi = string.Join(",", recipients.ToArray());

			return await GetPriceAsync(recipientsSemi, message);
		}

		private async Task<XDocument> GetXDocumentAsync(Url url)
		{
			XDocument response;

			var errorResponse = new XDocument(
				new XElement("result",
					new XElement("status", "error"),
					new XElement("message", "")));

			var errorMessage = errorResponse.Element("result")?.Element("message");

			try
			{
				response = await PrepareUrl(url)
					.GetXDocumentAsync();
			}
			catch (FlurlHttpException ex)
			{
				errorMessage?.SetValue(ex.Message);
				response = errorResponse;
			}
			catch (Exception ex)
			{
				errorMessage?.SetValue(ex.Message);
				response = errorResponse;
			}

			return response;
		}

		/// <summary>
		/// Prepares the URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		private Url PrepareUrl(Url url)
		{
			// Sort query params collection by the values
			var queryParamsValuesArray = (from p in url.QueryParams
										  orderby p.Value
										  select (string)p.Value).ToArray();

			// Make "params;params" string
			var queryParamsString = string.Join(";", queryParamsValuesArray);

			// Generate sign hash
			var hash = GetHash(MD5.Create(), GetHash(SHA1.Create(), $"{queryParamsString};{_apiKey}"));

			// Return prepared string with sign
			return url.SetQueryParam("sign", hash);
		}

		/// <summary>
		/// Gets the hash.
		/// </summary>
		/// <param name="hashString">The hash string.</param>
		/// <param name="text">The text.</param>
		private static string GetHash(HashAlgorithm hashString, string text)
		{
			var bytes = Encoding.UTF8.GetBytes(text);
			return hashString.ComputeHash(bytes).Aggregate("", (current, num) => current + $"{num:x2}");
		}

		/// <summary>
		/// Gets the double.
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="key">The key.</param>
		/// <param name="result">The result.</param>
		internal static bool TryGetDouble(XContainer from, string key, out double result)
		{
			result = 0;

			var element = from.Element(key);
			try
			{
				if (element?.Value == null)
					return false;

				//Try parsing in the current culture
				if (double.TryParse(element.Value, NumberStyles.Any, CultureInfo.CurrentCulture, out result) ||
					double.TryParse(element.Value, NumberStyles.Any, new CultureInfo("en-US"),
						out result) ||
					double.TryParse(element.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))

					return true;

				result = 0;
				return false;
			}
			catch (FormatException)
			{
				return false;
			}
		}
	}
}