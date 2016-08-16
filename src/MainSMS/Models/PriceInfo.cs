// Licensed under the GPL License, Version 3.0. See LICENSE in the git repository root for license information.

using System.Collections.Generic;
using System.Xml.Linq;

namespace MainSMS
{
	/// <summary>Response with message price information.</summary>
	/// <seealso cref="BaseResponse" />
	public class PriceInfo : BaseResponse
	{
		/// <summary>Initializes a new instance of the <see cref="PriceInfo" /> class.</summary>
		/// <param name="response">The response.</param>
		public PriceInfo(XContainer response) : base(response)
		{
			if (!ErrorsHandler())
				return;

			Balance = ExtractDouble("balance");
			Price = ExtractDouble("price");
			MessageCount = ExtractInt("count");
			PartsCount = ExtractInt("parts");
			Recipients = ExtractStringsFromArray("recipients");
		}

		/// <summary>Gets the balance.</summary>
		/// <value>The balance.</value>
		public double Balance { get; private set; }

		/// <summary>Gets the message count.</summary>
		/// <value>The message count.</value>
		public int MessageCount { get; private set; }

		/// <summary>Gets the parts count in the one message.</summary>
		/// <value>The parts count count in the one message.</value>
		public int PartsCount { get; private set; }

		/// <summary>Gets the price.</summary>
		/// <value>The price.</value>
		public double Price { get; private set; }

		/// <summary>Gets the recipients.</summary>
		/// <value>The recipients.</value>
		public IEnumerable<string> Recipients { get; }
	}
}