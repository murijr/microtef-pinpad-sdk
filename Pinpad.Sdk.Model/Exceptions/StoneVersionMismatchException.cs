﻿using System;
namespace Pinpad.Sdk.Model.Exceptions
{
	/// <summary>
	/// Exception for when the PinPad's Stone Version is below the command's minimum or when attempting to send a Stone command to a PinPad wihtout Stone App
	/// </summary>
	public class StoneVersionMismatchException : PinpadException 
	{
		public string Reason { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Exception Message</param>
		/// <param name="inner">Inner Exception</param>
		public StoneVersionMismatchException(string reason = null, string message = null, Exception inner = null) : base(null, null, message, inner) 
		{
			this.Reason = reason;
		}
	}
}
