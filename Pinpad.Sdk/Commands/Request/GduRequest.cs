﻿using Pinpad.Sdk.PinpadProperties.Refactor;
using Pinpad.Sdk.PinpadProperties.Refactor.Command;
using Pinpad.Sdk.PinpadProperties.Refactor.Formatter;
using Pinpad.Sdk.PinpadProperties.Refactor.Parser;
using Pinpad.Sdk.PinpadProperties.Refactor.Property;
using System;

namespace Pinpad.Sdk.Commands
{
	/// <summary>
	/// This command is the GDU request.
	/// The CDU command btains the KSN ("Key Serial Number") of a specific index on acquirer table.
	/// This operation depends on encryption methods such as: DUKPT/DES/PIN or DUKPT/TDES/PIN.
	/// </summary>
	internal sealed class GduRequest : BaseCommand
	{
		/// <summary>
		/// Command name. In this case, GDU that is an acronym to "Get DUKPT Serial Number".
		/// </summary>
		public override string CommandName { get { return "GDU"; } }
		/// <summary>
		/// Length of the first region of the command
		/// </summary>
		public RegionProperty CMD_LEN1 { get; private set; }
		/// <summary>
		/// Encryption method. Corresponds to the Key Management Mode + Cryptography Mode.
		/// </summary>
		public TextProperty<CryptographyMethod> GDU_METHOD { get; private set; }
		/// <summary>
		/// GDU_METHOD index.
		/// </summary>Mante
		public FixedLengthProperty<Nullable<int>> GDU_IDX { get; private set; }

		/// <summary>
		/// Creates GDU request command with all properties and regions.
		/// </summary>
		public GduRequest ()
		{
			this.CMD_LEN1 = new RegionProperty("CMD_LEN", 3);
			this.GDU_METHOD = new TextProperty<CryptographyMethod>("GDU_METHOD", false, 
                CryptographyMethod.StringFormatter, CryptographyMethod.CustomStringParser);
			this.GDU_IDX = new FixedLengthProperty<Nullable<int>>("GDU_IDX", 2, false, 
                StringFormatter.IntegerStringFormatter, StringParser.IntegerStringParser);

			this.StartRegion(this.CMD_LEN1);
			{
				this.AddProperty(this.GDU_METHOD);
				this.AddProperty(this.GDU_IDX);
			}
			this.EndLastRegion();
		}
	}
}
