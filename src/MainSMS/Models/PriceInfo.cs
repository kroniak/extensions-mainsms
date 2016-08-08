using System.Collections.Generic;
using System.Xml.Linq;

namespace MainSMS
{
	/// <summary>
	/// Reponse with balance of account.
	/// </summary>
	/// <seealso cref="BaseResponse" />
	public class PriceInfo : BaseResponse
	{
		/// <summary>
		/// Gets the balance.
		/// </summary>
		/// <value>
		/// The balance.
		/// </value>
		public double Balance { get; private set; }

		/// <summary>
		/// Gets the recipients.
		/// </summary>
		/// <value>
		/// The recipients.
		/// </value>
		public List<string> Recipients { get; }

		/// <summary>
		/// Gets the message count.
		/// </summary>
		/// <value>
		/// The message count.
		/// </value>
		public int MessageCount { get; private set; }

		/// <summary>
		/// Gets the parts count in the one message.
		/// </summary>
		/// <value>
		/// The parts count count in the one message.
		/// </value>
		public int PartsCount { get; private set; }

		/// <summary>
		/// Gets the price.
		/// </summary>
		/// <value>
		/// The price.
		/// </value>
		public double Price { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Balance"/> class.
		/// </summary>
		/// <param name="response">The response.</param>
		public PriceInfo(XContainer response) : base(response)
		{
			if (!ErrorsHandler())
				return;

			double temporary;
			if (MainSmsClient.TryGetDouble(Response, "balance", out temporary))
			{
				Balance = temporary;
			}
			else
			{
				ErrorMessage = "Cannot convert values from string";
				Status = "error";
				return;
			}

			if (MainSmsClient.TryGetDouble(Response, "price", out temporary))
			{
				Price = temporary;
			}
			else
			{
				ErrorMessage = "Cannot convert values from string";
				Status = "error";
				return;
			}

			var count = Response.Element("count");
			if (count?.Attribute("type")?.Value == "integer")
				MessageCount = int.Parse(count.Value);
			else
			{
				ErrorMessage = "Cannot convert values from string";
				Status = "error";
				return;
			}

			var parts = Response.Element("parts");
			if (parts?.Attribute("type")?.Value == "integer")
				PartsCount = int.Parse(parts.Value);
			else
			{
				ErrorMessage = "Cannot convert values from string";
				Status = "error";
				return;
			}

			Recipients = new List<string>();

			var xElements = Response.Element("recipients")?.Elements();

			if (xElements == null)
			{
				Status = "error";
				ErrorMessage = "Phone list is empty";
				return;
			}

			foreach (var rec in xElements)
			{
				Recipients.Add(rec.Value);
			}
		}
	}
}