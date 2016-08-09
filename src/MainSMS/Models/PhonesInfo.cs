using System.Collections.Generic;
using System.Xml.Linq;

namespace MainSMS
{
	/// <summary>
	///     Response with info of the phones.
	/// </summary>
	/// <seealso cref="BaseResponse" />
	public class PhonesInfo : BaseResponse
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="PhonesInfo" /> class.
		/// </summary>
		/// <param name="response">The response.</param>
		public PhonesInfo(XContainer response) : base(response)
		{
			if (!ErrorsHandler()) return;

			Phones = new List<PhoneInfo>();
			var xElements = Response.Element("info")?.Elements();

			if (xElements == null)
			{
				Status = "error";
				ErrorMessage = "Phone list is empty";
				return;
			}

			foreach (var info in xElements)
			{
				var phoneInfo = new PhoneInfo
				{
					Code = info?.Element("code")?.Value,
					OperatorName = info?.Element("name")?.Value,
					Phone = info?.Element("phone")?.Value,
					Region = info?.Element("region")?.Value
				};

				Phones.Add(phoneInfo);
			}
		}

		/// <summary>
		///     Gets the phones information.
		/// </summary>
		/// <value>
		///     The phones information.
		/// </value>
		public List<PhoneInfo> Phones { get; }
	}
}