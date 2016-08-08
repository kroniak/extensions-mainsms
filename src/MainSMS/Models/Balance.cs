using System.Xml.Linq;

namespace MainSMS
{
	/// <summary>
	/// Reponse with balance of account.
	/// </summary>
	/// <seealso cref="BaseResponse" />
	public class Balance : BaseResponse
	{
		/// <summary>
		/// The balance
		/// </summary>
		public readonly string BalanceValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="Balance"/> class.
		/// </summary>
		/// <param name="response">The response.</param>
		public Balance(XContainer response) : base (response)
		{
			try
			{
				if (ErrorsHandler())
				{
					BalanceValue = Response.Element("balance")?.Value;
				}
			}
			catch
			{
				Status = "error";
				ErrorMessage = "Неизвестная ошибка, возможно проблемы с соединением.";
				Error = "-1";
			}
		}
	}
}