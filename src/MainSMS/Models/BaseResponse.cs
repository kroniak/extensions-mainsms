using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace MainSMS
{
	/// <summary>
	///     Base abstract class for XML response.
	/// </summary>
	public abstract class BaseResponse
	{
		/// <summary>
		///     The response.
		/// </summary>
		protected readonly XContainer Response;

		/// <summary>
		///     Initializes a new instance of the <see cref="BaseResponse" /> class.
		/// </summary>
		/// <param name="response">The response.</param>
		protected BaseResponse(XContainer response)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			Response = response.Element("result");
			Url = Response?.Element("url")?.Value;
		}

		/// <summary>
		///     Gets the error code.
		/// </summary>
		/// <value>
		///     The error code.
		/// </value>
		public string ErrorCode { get; private set; }

		/// <summary>
		///     Gets the error message.
		/// </summary>
		/// <value>
		///     The error message.
		/// </value>
		public string ErrorMessage { get; protected set; }

		/// <summary>
		///     Gets the status of the request.
		/// </summary>
		/// <value>
		///     The status.
		/// </value>
		public string Status { get; protected set; }

		/// <summary>
		///     Gets the URL.
		/// </summary>
		/// <value>
		///     The URL.
		/// </value>
		public string Url { get; private set; }

		/// <summary>
		///     Errors handler.
		/// </summary>
		protected bool ErrorsHandler()
		{
			if (Response?.Element("status")?.Value == "success")
			{
				Status = "success";
				return true;
			}

			Status = "error";
			ErrorCode = Response?.Element("error")?.Value;
			ErrorMessage = Response?.Element("message")?.Value;
			return false;
		}

		/// <summary>
		///     Extracts the strings from XML array.
		/// </summary>
		/// <param name="key">The key.</param>
		protected IEnumerable<string> ExtractStringsFromArray(string key)
		{
			var xElements = Response.Element(key)?.Elements();

			if (xElements != null)
				return xElements.Select(rec => rec.Value);

			Status = "error";
			ErrorMessage = $"List {key} is empty";
			return null;
		}

		/// <summary>
		///     Extracts the ints from XML array.
		/// </summary>
		/// <param name="key">The key.</param>
		protected IEnumerable<int> ExtractIntsFromArray(string key)
		{
			var xElements = Response.Element(key)?.Elements();

			if (xElements != null)
				return xElements.Select(rec => int.Parse(rec.Value));

			Status = "error";
			ErrorMessage = $"List {key} is empty";
			return null;
		}

		/// <summary>
		///     Extract the double from value.
		/// </summary>
		/// <param name="key">The key.</param>
		protected double ExtractDouble(string key)
		{
			var element = Response.Element(key);

			try
			{
				double result;

				//Try parsing in the current culture
				if (double.TryParse(element?.Value, NumberStyles.Any, CultureInfo.CurrentCulture, out result) ||
				    double.TryParse(element?.Value, NumberStyles.Any, new CultureInfo("en-US"),
					    out result) ||
				    double.TryParse(element?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))

					return result;

				ErrorMessage = $"Cannot convert value {key} from string";
				Status = "error";
				return 0;
			}
			catch (FormatException)
			{
				ErrorMessage = $"Cannot convert value {key} from string";
				Status = "error";
				return 0;
			}
		}

		/// <summary>
		///     Extracts the int from value.
		/// </summary>
		/// <param name="key">The key.</param>
		protected int ExtractInt(string key)
		{
			var count = Response.Element(key);

			try
			{
				if (count?.Attribute("type")?.Value == "integer")
					return int.Parse(count.Value);

				ErrorMessage = $"Cannot convert value {key} from string";
				Status = "error";
				return 0;
			}
			catch (FormatException)
			{
				ErrorMessage = $"Cannot convert value {key} from string";
				Status = "error";
				return 0;
			}
		}
	}
}