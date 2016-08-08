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
		/// The error
		/// </summary>
		public string Error;

		/// <summary>
		/// The error message
		/// </summary>
		public string ErrorMessage;

		/// <summary>
		/// The status
		/// </summary>
		public string Status;

		/// <summary>
		/// The responce
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
			Error = Response?.Element("error")?.Value;
			ErrorMessage = Response?.Element("message")?.Value;
			return false;
		}
	}
}