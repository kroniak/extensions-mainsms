using System.Collections.Generic;
using System.Xml.Linq;

namespace MainSMS
{
	/// <summary>
	///     Response with messages status.
	/// </summary>
	/// <seealso cref="BaseResponse" />
	public class MessagesInfo : BaseResponse
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="MessagesInfo" /> class.
		/// </summary>
		/// <param name="response">The response.</param>
		public MessagesInfo(XContainer response) : base(response)
		{
			if (!ErrorsHandler()) return;

			Statuses = new Dictionary<string, string>();

			var xElements = Response.Element("messages")?.Elements();

			if (xElements == null)
			{
				Status = "error";
				ErrorMessage = "Statuses list is empty";
				return;
			}

			foreach (var element in xElements)
				if (element?.Value != null)
					Statuses.Add(element.Name.LocalName.Replace("id", ""), element.Value);
		}

		/// <summary>
		///     Gets the statuses of the messages.
		/// </summary>
		/// <value>
		///     The statuses.
		/// </value>
		public Dictionary<string, string> Statuses { get; }
	}
}