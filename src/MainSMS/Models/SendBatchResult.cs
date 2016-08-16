// Licensed under the GPL License, Version 3.0. See LICENSE in the git repository root for license information.

using System.Xml.Linq;

namespace MainSMS
{
	/// <summary>Response with result of the batch sending.</summary>
	/// <seealso cref="BaseResponse" />
	public class SendBatchResult : BaseResponse
	{
		/// <summary>Initializes a new instance of the <see cref="SendBatchResult" /> class.</summary>
		/// <param name="response">The response.</param>
		public SendBatchResult(XContainer response) : base(response)
		{
			if (!ErrorsHandler())
				return;

			Price = ExtractDouble("cost");
			RecipientsCount = ExtractInt("phones");
			PartsCount = ExtractInt("parts");
		}

		/// <summary>Gets the parts count in the one message.</summary>
		/// <value>The parts count count in the one message.</value>
		public int PartsCount { get; private set; }

		/// <summary>Gets the price.</summary>
		/// <value>The price.</value>
		public double Price { get; private set; }

		/// <summary>Gets the message count.</summary>
		/// <value>The message count.</value>
		public int RecipientsCount { get; private set; }
	}
}