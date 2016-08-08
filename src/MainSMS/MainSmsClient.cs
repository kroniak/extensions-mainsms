using System;
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
				return TestMode ? TestUrl : _apiUrl;
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
		public bool TestMode { private get; set; }

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
		/// Gets the balance.
		/// </summary>
		public async Task<Balance> GetBalanceAsync()
		{
			var url = ApiUrl
				.AppendPathSegment("balance");

			var responce = await GetXDocumentAsync(url);

			return new Balance(responce);
		}

		private async Task<XDocument> GetXDocumentAsync(Url url)
		{
			XDocument response;

			var errorResponse = new XDocument(
				new XElement("result",
					new XElement("status", "error"),
					new XElement("message","")));

			var errorMessage = errorResponse.Element("result").Element("message");

			try
			{
				response = await PrepareUrl(url)
					.GetXDocumentAsync();
			}
			catch (FlurlHttpException ex)
			{
				errorMessage.SetValue(ex.Message);
				response = errorResponse;
			}
			catch (Exception ex)
			{
				errorMessage.SetValue(ex.Message);
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
	}
}