// Licensed under the GPL License, Version 3.0. See LICENSE in the git repository root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Flurl;
using Flurl.Http;
using Flurl.Http.Xml;
#if NETSTANDARD1_4
using System.Text.Encodings.Web;
#elif NET45
using System.Web;
#endif

namespace MainSMS
{
	/// <summary>Main client class to MainSMS API.</summary>
	public class MainSmsClient
	{
		// Default  type for response
		private const string ResponseType = "xml";

		private readonly string _apiKey;

		private string _apiUrl;

		/// <summary>Gets or sets a value indicating whether [test mode].</summary>
		/// <value>
		/// <c>true</c> if [test mode]; otherwise, <c>false</c>.
		/// </value>
		private bool _testMode;

		/// <summary>Initializes a new instance of the <see cref="MainSmsClient" /> class.</summary>
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
				.SetQueryParams(new {project, format = ResponseType});
		}

		// Default API URL
		private string ApiUrl
		{
			get { return _testMode ? TestUrl : _apiUrl; }
			set { _apiUrl = value; }
		}

		/// <summary>Gets or sets the test URL.</summary>
		/// <value>The test URL.</value>
		public string TestUrl { private get; set; }

		/// <summary>Cancel the delayed messages asynchronous.</summary>
		/// <param name="messagesIds">The messages ids separated by semicolons.</param>
		public async Task<CancelResult> CancelAsync(string messagesIds)
		{
			var url = ApiUrl
				.AppendPathSegment("cancel")
				.SetQueryParam("messages_id", messagesIds);

			var responce = await GetXDocumentAsync(url);

			return new CancelResult(responce);
		}

		/// <summary>Cancel the delayed message asynchronous.</summary>
		/// <param name="messageId">The message id.</param>
		public async Task<CancelResult> CancelAsync(int messageId)
			=> await CancelAsync(messageId.ToString());

		/// <summary>Cancel the delayed messages asynchronous.</summary>
		/// <param name="messagesIds">The messages ids.</param>
		public async Task<CancelResult> CancelAsync(IEnumerable<int> messagesIds)
		{
			var messagesIdsSemi = string.Join(",", messagesIds.ToArray());

			return await CancelAsync(messagesIdsSemi);
		}

		/// <summary>Encodes the specified message.</summary>
		/// <param name="message">The message.</param>
		private static string Encode(string message)
		{
#if NETSTANDARD1_4
			return UrlEncoder.Default.Encode(message);
#elif NET45
			return HttpUtility.UrlEncode(message);
#endif
		}

		/// <summary>Gets the balance.</summary>
		public async Task<BalanceInfo> GetBalanceAsync()
		{
			var url = ApiUrl
				.AppendPathSegment("balance");

			var responce = await GetXDocumentAsync(url);

			return new BalanceInfo(responce);
		}

		/// <summary>Gets the hash.</summary>
		/// <param name="hashString">The hash string.</param>
		/// <param name="text">The text.</param>
		private static string GetHash(HashAlgorithm hashString, string text)
		{
			var bytes = Encoding.UTF8.GetBytes(text);
			return hashString.ComputeHash(bytes).Aggregate("", (current, num) => current + $"{num:x2}");
		}

		/// <summary>Gets the phones information asynchronous.</summary>
		/// <param name="phones">The phones separated by semicolons.</param>
		public async Task<PhonesInfo> GetInfoAsync(string phones)
		{
			var url = ApiUrl
				.AppendPathSegment("info")
				.SetQueryParam("phones", phones);

			var responce = await GetXDocumentAsync(url);

			return new PhonesInfo(responce);
		}

		/// <summary>Gets the phones information asynchronous.</summary>
		/// <param name="phones">The phones.</param>
		public async Task<PhonesInfo> GetInfoAsync(IEnumerable<string> phones)
		{
			var phonesSemi = string.Join(",", phones.ToArray());

			return await GetInfoAsync(phonesSemi);
		}

		/// <summary>Gets the price asynchronous.</summary>
		/// <param name="recipients">The recipients phones.</param>
		/// <param name="message">The message in UTF8.</param>
		public async Task<PriceInfo> GetPriceAsync(string recipients, string message)
		{
			var url = ApiUrl
				.AppendPathSegment("price")
				.SetQueryParam("recipients", recipients)
				.SetQueryParam("message", Encode(message));

			var responce = await GetXDocumentAsync(url);

			return new PriceInfo(responce);
		}

		/// <summary>Gets the price asynchronous.</summary>
		/// <param name="recipients">The recipients phones.</param>
		/// <param name="message">The message in UTF8.</param>
		public async Task<PriceInfo> GetPriceAsync(IEnumerable<string> recipients, string message)
		{
			var recipientsSemi = string.Join(",", recipients.ToArray());

			return await GetPriceAsync(recipientsSemi, message);
		}

		/// <summary>Gets the statuses asynchronous.</summary>
		/// <param name="messageId">The message id.</param>
		public async Task<MessagesInfo> GetStatusAsync(int messageId)
			=> await GetStatusesAsync(messageId.ToString());

		/// <summary>Gets the statuses asynchronous.</summary>
		/// <param name="messagesIds">The messages ids separated by semicolons.</param>
		public async Task<MessagesInfo> GetStatusesAsync(string messagesIds)
		{
			var url = ApiUrl
				.AppendPathSegment("status")
				.SetQueryParam("messages_id", messagesIds);

			var responce = await GetXDocumentAsync(url);

			return new MessagesInfo(responce);
		}

		/// <summary>Gets the statuses asynchronous.</summary>
		/// <param name="messagesIds">The messages ids.</param>
		public async Task<MessagesInfo> GetStatusesAsync(IEnumerable<int> messagesIds)
		{
			var messagesIdsSemi = string.Join(",", messagesIds.ToArray());

			return await GetStatusesAsync(messagesIdsSemi);
		}

		/// <summary>Gets the test client.</summary>
		public MainSmsClient GetTestClient()
		{
			_testMode = true;
			return this;
		}

		/// <summary>Gets the test client.</summary>
		/// <param name="testUrl">The test URL.</param>
		public MainSmsClient GetTestClient(string testUrl)
		{
			TestUrl = testUrl;
			return GetTestClient();
		}

		/// <summary>Gets the x document asynchronous.</summary>
		/// <param name="url">The URL.</param>
		/// <param name="prepareMethodCall">The prepare method call.</param>
		/// <param name="queryParams">The query parameters.</param>
		private async Task<XDocument> GetXDocumentAsync(Url url, Action<Url> prepareMethodCall = null,
			string queryParams = null)
		{
			XDocument response;

			var errorResponse = new XDocument(
				new XElement("result",
					new XElement("status", "error"),
					new XElement("message", "")));

			var errorMessage = errorResponse.Element("result")?.Element("message");

			try
			{
				// add sign to url
				if (prepareMethodCall == null)
					prepareMethodCall = delegate { PrepareUrl(url); };

				prepareMethodCall(url);

				string resultUrl;
				// add query params if needed
				if (!string.IsNullOrEmpty(queryParams))
					resultUrl = url + "&" + queryParams;
				else
					resultUrl = url;

				// add prepared url to response to testing
				errorResponse.Element("result")?.Add(new XElement("url", resultUrl));

				// do HTTP request
				response = await resultUrl.GetXDocumentAsync();

				// add prepared url to response to testing
				response.Element("result")?.Add(new XElement("url", resultUrl));
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

		/// <summary>Prepares the URL.</summary>
		/// <param name="url">The URL.</param>
		private void PrepareUrl(Url url)
		{
			// Sort query params collection by the values
			var queryParamsValuesArray = (from p in url.QueryParams
										  orderby p.Name
										  select (string) p.Value).ToArray();

			// Make "params;params" string
			var queryParamsString = string.Join(";", queryParamsValuesArray);

			// Generate sign hash
			var hash = GetHash(MD5.Create(), GetHash(SHA1.Create(), $"{queryParamsString};{_apiKey}"));

			// Add sign to url
			url.SetQueryParam("sign", hash);
		}

		/// <summary>Prepares the URL.</summary>
		/// <param name="url">The URL.</param>
		private void PrepareUrlForBatch(Url url)
		{
			PrepareUrl(url);

			// remove prepared string from real URL
			url.RemoveQueryParam("messages");
		}

		/// <summary>Sends the message asynchronous.</summary>
		/// <param name="recipients">The recipients phones.</param>
		/// <param name="message">The message in UTF8.</param>
		/// <param name="sender">The sender name from 5 to 11 chars, latin and numeric only.</param>
		/// <param name="startDateTime">Send message at this time in your time zone.</param>
		/// <param name="testMode">If set to <c>true</c> [test mode].</param>
		public async Task<SendResult> SendAsync(string recipients, string message, string sender = null,
			DateTime startDateTime = default(DateTime),
			bool testMode = false)
		{
			var url = ApiUrl
				.AppendPathSegment("send")
				.SetQueryParam("recipients", recipients)
				.SetQueryParam("message", Encode(message))
				.SetQueryParam("sender", sender)
				.SetQueryParam("run_at", startDateTime > DateTime.Now ? startDateTime.ToString("d.M.yyyy H:m") : null)
				.SetQueryParam("test", testMode ? "1" : "0");

			var responce = await GetXDocumentAsync(url);

			return new SendResult(responce);
		}

		/// <summary>Sends the message asynchronous.</summary>
		/// <param name="recipients">The recipients phones.</param>
		/// <param name="message">The message in UTF8.</param>
		/// <param name="sender">The sender name from 5 to 11 chars, latin and numeric only.</param>
		/// <param name="startDateTime">Send message at this time in your time zone.</param>
		/// <param name="testMode">If set to <c>true</c> [test mode].</param>
		public async Task<SendResult> SendAsync(IEnumerable<string> recipients, string message, string sender = null,
			DateTime startDateTime = default(DateTime),
			bool testMode = false)
		{
			var recipientsSemi = string.Join(",", recipients.ToArray());

			return await SendAsync(recipientsSemi, message, sender, startDateTime, testMode);
		}

		/// <summary>Sends the messages in the batch asynchronous.</summary>
		/// <param name="messages">The recipients phones (key) and messages in UTF8 (value) Dictionary.</param>
		/// <param name="sender">The sender name from 5 to 11 chars, latin and numeric only.</param>
		/// <param name="testMode">If set to <c>true</c> [test mode].</param>
		public async Task<SendBatchResult> SendBatchAsync(IDictionary<string, string> messages, string sender = null,
			bool testMode = false)
		{
			if (messages == null)
				throw new ArgumentNullException(nameof(messages));
			if (messages.Count == 0)
				throw new ArgumentException(nameof(messages), $"{nameof(messages)} is empty.");

			// prepate messages param string
			var messagesParam = "";
			var messagesForHash = "";
			var i = 0;
			foreach (var message in messages)
			{
				messagesParam = $"{messagesParam}messages[{i}][phone]={message.Key}&messages[{i}][text]={Encode(message.Value)}&";
				messagesForHash = string.Join(",", messagesForHash, message.Key, message.Value);
				i++;
			}

			// remove first [,] and last [&] char
			messagesParam = messagesParam.Remove(messagesParam.Length - 1);
			if (messagesForHash.StartsWith(","))
				messagesForHash = messagesForHash.Substring(1);

			var url = ApiUrl
				.Replace("message", "batch")
				.AppendPathSegment("send")
				.SetQueryParam("messages", messagesForHash)
				.SetQueryParam("sender", sender)
				.SetQueryParam("test", testMode ? "1" : "0");

			var responce = await GetXDocumentAsync(url, PrepareUrlForBatch, messagesParam);

			return new SendBatchResult(responce);
		}
	}
}