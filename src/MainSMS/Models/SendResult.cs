using System.Collections.Generic;
using System.Xml.Linq;

namespace MainSMS
{
	/// <summary>
	///     Response with result of send message operation.
	/// </summary>
	/// <seealso cref="PriceInfo" />
	public class SendResult : PriceInfo
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="SendResult" /> class.
		/// </summary>
		/// <param name="response">The response.</param>
		public SendResult(XContainer response) : base(response)
		{
			if (!ErrorsHandler())
				return;

			MessageIds = ExtractIntsFromArray("messages-id");
			TestMode = ExtractInt("test") == 1;
		}

		/// <summary>
		///     Gets the message ids.
		/// </summary>
		/// <value>
		///     The message ids.
		/// </value>
		public IList<int> MessageIds { get; }

		/// <summary>
		///     Gets a value indicating whether [test mode].
		/// </summary>
		/// <value>
		///     <c>true</c> if [test mode]; otherwise, <c>false</c>.
		/// </value>
		public bool TestMode { get; private set; }
	}
}