﻿using Pinpad.Sdk.Connection;
using Pinpad.Sdk.Display;
using Pinpad.Sdk.Display.Mapper;
using Pinpad.Sdk.Model.TypeCode;
using Pinpad.Sdk.EmvTable;
using Pinpad.Sdk.Model;
using Pinpad.Sdk.Mapper;
using Pinpad.Sdk.Transaction;
using PinPadSDK.Commands;
using PinPadSDK.Controllers.Tracks;
using PinPadSDK.Enums;
using PinPadSDK.PinPad;
using PinPadSDK.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResponseStatus = Pinpad.Sdk.Model.TypeCode.ResponseStatus;
using LegacyResponseStatus = PinPadSDK.Enums.ResponseStatus;
using PinPadSDK.Controllers.Tables;
using System.Diagnostics;
using PinPadSDK.Exceptions;
using Pinpad.Sdk.Exceptions;

namespace Pinpad.Sdk
{
	/// <summary>
	/// Pinpad controller, containing all methods needed to perform a transaction.
	/// </summary>
	public class PinpadController
	{
		// Costants
		internal const short STONE_ACQUIRER_NUMBER = 8;

		// Members
		/// <summary>
		/// Facade through which pinpad communication is made.
		/// </summary>
		private PinPadFacade pinpadFacade;
		
		/// <summary>
		/// Connection handler, is responsible for specifying the connection through which the pinpad will be looked for.
		/// </summary>
		public BasePinpadConnection PinpadConnection { get; set; }
		/// <summary>
		/// Display handler, responsible for presenting messages in a pinpad device.
		/// </summary>
		public IPinpadDisplay Display { get; set; }
		/// <summary>
		/// Pinpad Emv table handler, that is, responsible for all operations related to table (CAPK and AID tables) controlling.
		/// </summary>
		public IPinpadTable EmvTable { get; set; }
		/// <summary>
		/// Last status returned from a pinpad command.
		/// </summary>
		public ResponseStatus LastCommandStatus { get; private set; }
		/// <summary>
		/// Information about the pinpad device connected.
		/// </summary>
		public static PinpadInfo PinpadInfo { get; private set; }

		// Constructors
		/// <summary>
		/// Primary constructor.
		/// </summary>
		/// <param name="pinpadConnection">Connection through which the pinpad will be looked for.</param>
		public PinpadController(BasePinpadConnection pinpadConnection) : this(pinpadConnection, new PinpadDisplay(pinpadConnection), PinpadTable.GetInstance(pinpadConnection)) { }
		/// <summary>
		/// Alternative constructor, setter of all mandatory parameters and responsible for data validation.
		/// </summary>
		/// <param name="pinpadConnection">Connection through which the pinpad will be looked for.</param>
		/// <param name="pinpadDisplay">Display handler.</param>
		/// <param name="pinpadTable">Pinpad emv table handler.</param>
		internal PinpadController(BasePinpadConnection pinpadConnection, IPinpadDisplay pinpadDisplay, IPinpadTable pinpadTable)
		{
			if (pinpadConnection == null)
			{
				this.LastCommandStatus = ResponseStatus.InvalidParameter;
				throw new ArgumentNullException("pinpadConnection");
			}

			this.PinpadConnection = pinpadConnection;
			this.Display = pinpadDisplay;
			this.EmvTable = pinpadTable;
			this.pinpadFacade = new PinPadFacade(this.PinpadConnection.LegacyPinpadConnection);
			this.LastCommandStatus = ResponseStatus.Ok;
			PinpadController.PinpadInfo = new PinpadInfo(new PinPadInfos(this.pinpadFacade));
		}

		// Methods
		/// <summary>
		/// On Pinpad screen, alternates between "RETIRE O CARTÃO" and parameter 'message' received, until card removal.
		/// </summary>
		/// <param name="message">Message to be shown on pinpad screen. Must not exceed 16 characters. This message remains on Pinpad screen after card removal.</param>
		/// <param name="padding">Message alignment.</param>
		/// <returns></returns>
		public bool RemoveCard(string message, DisplayPaddingType padding)
		{
			RmcRequest request = new RmcRequest();
			
			// Align the message according to its alignment.
			PaddingType mappedPadding = DisplayPaddingMapper.MapPaddingType(padding);
			
			// Assemblies RMC command.
			request.RMC_MSG.Value = new SimpleMessage(message, mappedPadding);

			// Sends command and receive response
			GenericResponse response = null;
            while (response == null)
			{
				response = this.pinpadFacade.Communication.SendRequestAndReceiveResponse<GenericResponse>(request);
            }
			
			// Getting legacy response status code:
			LegacyResponseStatus legacyStatus = LegacyResponseStatus.ST_OK;

			// Mapping legacy status code into Pinpad.Sdk response status code.
			this.LastCommandStatus = ResponseStatusMapper.MapLegacyResponseStatus(legacyStatus);
			
			// Verifies if the command was executed.
			if (this.LastCommandStatus == ResponseStatus.Ok) { return true; }
			else { return false; }
		}
		/// <summary>
		/// Read basic card information, that is, brand id, card type, card primary account number (PAN), cardholder name and expiration date.
		/// </summary>
		/// <param name="transactionType">Transaction type, that is, debit/credit.</param>
		/// <returns>Card basic info.</returns>
		public CardEntry ReadCard(TransactionType transactionType, decimal amount)
		{
			LegacyResponseStatus status;
			CardEntry cardRead;

			do
			{
				status = PerformCardReading(transactionType, amount, out cardRead);
				this.LastCommandStatus = ResponseStatusMapper.MapLegacyResponseStatus(status);

				// EMV tables are incompatible. Recharging tables:
				if (status == LegacyResponseStatus.ST_TABVERDIF || 
					status == LegacyResponseStatus.ST_CARDAPPNAV)
				{
					// TODO: FAZER UM TRATAMENTO DESCENTE
					return null;
				}
				else if (status == LegacyResponseStatus.ST_TIMEOUT)
				{
					throw new PinpadDisconnectedException();
				}
				else if (status == LegacyResponseStatus.ST_TABERR)
				{
					throw new InvalidTableException("EMV table version could not be found.");
				}

			} while (status != LegacyResponseStatus.ST_OK && status != LegacyResponseStatus.ST_CANCEL);

			return cardRead;
		}
		private LegacyResponseStatus PerformCardReading(TransactionType transactionType, decimal amount, out CardEntry cardRead)
		{
			cardRead = new CardEntry();

			// Assembling GCR command request:
			GcrRequest request = new GcrRequest();

			request.GCR_ACQIDXREQ.Value = STONE_ACQUIRER_NUMBER;

			if (transactionType != TransactionType.Undefined)
			{
				request.GCR_APPTYPREQ.Value = (int)transactionType;
			}
			else
			{
				request.GCR_APPTYPREQ.Value = 99;
			}

			request.GCR_AMOUNT.Value = Convert.ToInt64(amount * 100);
			request.GCR_DATE_TIME.Value = DateTime.Now;

			// Retieving current EMV table version from pinpad:
			string emvTableVersion = this.EmvTable.GetEmvTableVersion();
			Debug.WriteLine("EMV table version: <{0}>", emvTableVersion);
			
			if (emvTableVersion == null)
			{
				// There's no table version, therefore tables cannot be reached.
				return LegacyResponseStatus.ST_TABERR;
			}

			// If it's a valid EMV table version, then adds it to the command:
			request.GCR_TABVER.Value = emvTableVersion;

			// Sending and receiving response.
			Debug.WriteLine("Sending GCR command <{0}>", request.CommandString);
			GcrResponse response = this.pinpadFacade.Communication.SendRequestAndReceiveResponse<GcrResponse>(request);
			if (response == null)
			{
				return LegacyResponseStatus.ST_TIMEOUT;
			}

			Debug.WriteLine("GCR response <{0}>.", response.RSP_STAT.Value);
			Debug.WriteLine("GCR raw response <{0}>.", response.CommandString);

			if (response.RSP_STAT.Value != LegacyResponseStatus.ST_OK)
			{
				return response.RSP_STAT.Value;
			}

			// Saving command response status:
			// Getting legacy response status code:
			LegacyResponseStatus legacyStatus = response.RSP_STAT.Value;
			// Mapping legacy status code into Pinpad.Sdk response status code.
			this.LastCommandStatus = ResponseStatusMapper.MapLegacyResponseStatus(legacyStatus);

			// Get card information and return it:
			cardRead = CardMapper.MapCardFromTracks(response);

			if (cardRead.Type == CardType.Emv)
			{
				this.EmvTable.RefreshFromPinpad();

                if (this.EmvTable.AidTable.Count <= 0)
                {
                    throw new InvalidTableException("AID table is empty.");
                }

				string brandId = cardRead.BrandId.ToString();
				var aidVar = this.EmvTable.AidTable.First(a => a.AidIndex == brandId);

				cardRead.ApplicationId = aidVar.ApplicationId;
				cardRead.BrandName = EmvTrackMapper.GetBrandByAid(cardRead.ApplicationId);
			}

			return LegacyResponseStatus.ST_OK;
		}
		/// <summary>
		/// If cardholder card needs password, than prompts it. Otherwise, nothing is done. 
		/// </summary>
		/// <param name="amount">Transaction amount in cents.</param>
		/// <param name="pan">Primary Account Number (PAN).</param>
		/// <param name="readingMode">Card type, defining how the card must be read.</param>
		/// <returns>An object Pin, containing pin block (cardholder password), Key Serial Number (KSN, determining pin block DUKPT encryption key) and if pin verification is online. </returns>
		public Pin ReadPassword(decimal amount, string pan = "", CardType readingMode = CardType.Emv)
		{
			Debug.WriteLine("Readig mode <{0}>.", readingMode);

			PinReader reader = new PinReader(this.pinpadFacade, readingMode);
			
			// Gets Pin:
			Pin pin = reader.Read(amount, pan);

			Debug.WriteLine("PIN read. Response <{0}>.", reader.CommandStatus);

			// Saving last command status:
			this.LastCommandStatus = reader.CommandStatus;

			return pin;
		}

		// Validations
		/// <summary>
		/// Responsible for validating mandatory parameters.
		/// </summary>
		/// <param name="pinpadConnection">Connection through which the pinpad will be looked for.</param>
		/// <param name="pinpadDisplay">Display handler.</param>
		/// <param name="pinpadTable">Pinpad emv table handler.</param>
		/// <exception cref="System.ArgumentNullException">Thrown when a parameter is null (specifies which is).</exception>
		public void Validate(BasePinpadConnection pinpadConnection, IPinpadDisplay pinpadDisplay, IPinpadTable pinpadTable)
		{
			if (pinpadConnection == null) { throw new ArgumentNullException("pinpadConnection"); }
			if (pinpadDisplay == null) { throw new ArgumentNullException("pinpadDisplay"); }
			if (pinpadTable == null) { throw new ArgumentNullException("pinpadTable"); }
		}
	}
}
