// Licensed under the GPL License, Version 3.0. See LICENSE in the git repository root for license information.

using System.Collections.Generic;
using System.Xml.Linq;

namespace MainSMS
{
	/// <summary>Response with cancel messages result.</summary>
	/// <seealso cref="BaseResponse" />
	public class CancelResult : BaseResponse
	{
		/// <summary>Initializes a new instance of the <see cref="CancelResult" /> class.</summary>
		/// <param name="response">The response.</param>
		public CancelResult(XContainer response) : base(response)
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

		/// <summary>Gets the statuses of the messages.</summary>
		/// <value>The statuses.</value>
		public Dictionary<string, string> Statuses { get; }
	}
}