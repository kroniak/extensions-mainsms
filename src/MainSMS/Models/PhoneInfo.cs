﻿namespace MainSMS
{
	/// <summary>
	/// Information about one phone number.
	/// </summary>
	public class PhoneInfo
	{
		/// <summary>
		/// Gets the phone.
		/// </summary>
		/// <value>
		/// The phone.
		/// </value>
		public string Phone { get; internal set; }

		/// <summary>
		/// Gets the code.
		/// </summary>
		/// <value>
		/// The code.
		/// </value>
		public string Code { get; internal set; }

		/// <summary>
		/// Gets the region.
		/// </summary>
		/// <value>
		/// The region.
		/// </value>
		public string Region { get; internal set; }

		/// <summary>
		/// Gets the name of the operator.
		/// </summary>
		/// <value>
		/// The name of the operator.
		/// </value>
		public string OperatorName { get; internal set; }
	}
}