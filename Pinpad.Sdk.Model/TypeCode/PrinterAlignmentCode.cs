﻿namespace Pinpad.Sdk.Model
{
	/// <summary>
	/// Enumerator for PRT string alignment
	/// Since undefined is 0 every value will be the actual code plus 1
	/// </summary>
	public enum PrinterAlignmentCode
	{
		/// <summary>
		/// Null
		/// </summary>
		Undefined = 0,
		/// <summary>
		/// Left alignment
		/// </summary>
		Left = 1,
		/// <summary>
		/// Center alignment
		/// </summary>
		Center = 2,
		/// <summary>
		/// Right alignment
		/// </summary>
		Right = 3
	}
}
