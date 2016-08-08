using System;
using System.Xml.Linq;

namespace MainSMS
{
	/// <summary>
	/// Base abstract class for XML response.
	/// </summary>
	public abstract class BaseResponse
	{
		/// <summary>
		/// Gets the error code.
		/// </summary>
		/// <value>
		/// The error code.
		/// </value>
		public string ErrorCode { get; private set; }

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value>
		/// The error message.
		/// </value>
		public string ErrorMessage { get; protected set; }

		/// <summary>
		/// Gets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		public string Status { get; protected set; }

		/// <summary>
		/// The response.
		/// </summary>
		protected readonly XContainer Response;

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseResponse"/> class.
		/// </summary>
		/// <param name="response">The response.</param>
		protected BaseResponse(XContainer response)
		{
			if (response == null) throw new ArgumentNullException(nameof(response));
			
			Response = response.Element("result");
		}

		/// <summary>
		/// Errors handler.
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
	}
}