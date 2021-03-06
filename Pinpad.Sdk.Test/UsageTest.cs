﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microtef.CrossPlatform;
using NUnit.Framework;
using Pinpad.Sdk.Commands;
using Pinpad.Sdk.Model;
using Pinpad.Sdk.PinpadProperties.Refactor.Property;
using Pinpad.Sdk.Test.Mockers;
using Pinpad.Sdk.Model.TypeCode;

namespace Pinpad.Sdk.Test
{
//#if !Release && !Debug
    [TestFixture]
    public class UsageTest
    {
        [SetUp]
        public void Setup()
        {
            Microtef.Platform.Desktop.DesktopInitializer.Initialize();
        }

        [Test]
        public void GCD_test()
        {
            GcdRequest r = new GcdRequest();
            r.SPE_MSGIDX.Value = KeyboardMessageCode.AskForSecurityCode;
            r.SPE_MINDIG.Value = 1;
            r.SPE_MAXDIG.Value = 2;

            Debug.WriteLine((int)r.SPE_MSGIDX.Value);
            Debug.WriteLine(r.CommandString);
        }
        [Test]
        public void Gertec_EX07_test()
        {
            // testes:
            //this.mockedPinpadConnection = new MockedPinpadConnection();

            // prod:
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();

            PinpadCommunication comm = new PinpadCommunication(conn);

            GciGertecRequest request = new GciGertecRequest();

            request.NumericInputType.Value = KeyboardNumberFormat.Decimal;
            request.TextInputType.Value = KeyboardTextFormat.None;
            request.LabelFirstLine.Value = FirstLineLabelCode.TypeNumber;
            request.LabelSecondLine.Value = SecondLineLabelCode.GasPump;
            request.MaximumCharacterLength.Value = 1;
            request.MinimumCharacterLength.Value = 10;
            request.TimeOut.Value = 60;
            request.TimeIdle.Value = 0;

            Debug.WriteLine(request.CommandString);

            GertecEx07Response response = comm
                      .SendRequestAndReceiveResponse<GertecEx07Response>(request);

            if (response != null)
            {
                Debug.WriteLine("Response status: " + response.RSP_STAT.Value);
                Debug.WriteLine("Value typed: " + response.RSP_RESULT.Value);
            }
            else
            {
                Debug.WriteLine("Resposta nula. Cancelamento ou timeout.");
            }
        }
        [Test]
        public void Keyboard_getNumber_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();

            PinpadFacade facade = new PinpadFacade(conn);

            facade.Display.ShowMessage("ola", "tudo bom?", DisplayPaddingType.Center);

            string pump = facade.Keyboard.GetNumericInput(
                      FirstLineLabelCode.TypeNumber,
                      SecondLineLabelCode.GasPump,
                      1,
                      15,
                      15);

            if (pump == null)
            {
                Debug.WriteLine("Nao foi possivel ler um valor. Time out ou cancelamento.");
            }
            else
            {
                Debug.WriteLine("Valor digitado: " + pump);
            }
        }
        [Test]
        public void Keyboard_GetAmount_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();

            PinpadFacade pinpad = new PinpadFacade(conn);
            Nullable<decimal> amount = pinpad.Keyboard.GetAmount(AmountCurrencyCode.Yen);

            if (amount.HasValue)
            {
                Debug.WriteLine("Valor digitado: " + amount.Value);
            }
            else
            {
                Debug.WriteLine("Não foi possível ler o valor.");
            }
        }
        [Test]
        public void OPN_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();

            PinpadCommunication comm = new PinpadCommunication(conn);

            OpnRequest opn = new OpnRequest();
            OpnResponse opnResp = comm.SendRequestAndReceiveResponse<OpnResponse>(opn);

            DspRequest dsp = new DspRequest();
            dsp.DSP_MSG.Value = new SimpleMessageProperty("ola");

            GenericResponse r = comm.SendRequestAndReceiveResponse<GenericResponse>(dsp);
        }
        [Test]
        public void MultiplePinpads_test()
        {
            // Gets all connections:
            ICollection<IPinpadConnection> connections = CrossPlatformController
                      .PinpadFinder.FindAllDevices();

            if (connections == null || connections.Count <= 0)
            {
                Assert.IsTrue(false);
            }

            // Gets all pinpads:
            ICollection<IPinpadFacade> pinpads = new List<IPinpadFacade>();

            foreach (IPinpadConnection conn in connections)
            {
                pinpads.Add(new PinpadFacade(conn));
            }

            // Show corresponding COM port on the pinpad display:
            foreach (IPinpadFacade pinpad in pinpads)
            {
                pinpad.Display.ShowMessage(
                          "",
                          pinpad.Communication.ConnectionName,
                          DisplayPaddingType.Center);
            }

            // Closing conections...
            foreach (IPinpadFacade pinpad in pinpads)
            {
                pinpad.Communication.ClosePinpadConnection(
                          "Close " + pinpad.Communication.ConnectionName);
            }

            Assert.IsTrue(true);
        }
        [Test]
        public void OnePinpadFind_test()
        {
            for (int i = 0; i < 5; i++)
            {
                IPinpadConnection conn = PinpadConnectionProvider.GetFirst();
                PinpadFacade facade = new PinpadFacade(conn);

                facade.Display.ShowMessage(
                          "YAY! ^-^",
                          (i + 1).ToString(),
                          DisplayPaddingType.Center);

                facade.Communication
                            .ClosePinpadConnection("Fechando conexao (" + (i + 1).ToString() + ")");
            }
        }
        [Test]
        public void OnePinpadFind_with_portName_test()
        {
            for (int i = 0; i < 5; i++)
            {
                IPinpadConnection conn = PinpadConnectionProvider.GetAt("COM27");
                PinpadFacade facade = new PinpadFacade(conn);

                bool displayStatus = facade.Display.ShowMessage(
                          "YAY! ^-^",
                          (i + 1).ToString(),
                          DisplayPaddingType.Center);

                facade.Communication
                            .ClosePinpadConnection("Fechando conexao (" + (i + 1).ToString() + ")");
            }
        }
        [Test]
        public void Ping_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();
            PinpadFacade facade = new PinpadFacade(conn);

            facade.Display.ShowMessage("yay!", null, DisplayPaddingType.Center);
            facade.Communication.ClosePinpadConnection("adios");

            bool status = facade.Communication.Ping();
        }
        [Test]
        public void GetNumericValue_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();
            PinpadFacade facade = new PinpadFacade(conn);

            Nullable<int> value = facade.Keyboard.DataPicker.GetNumericValue(
                      "Data Picker",
                      false,
                      0,
                      5);

            Debug.WriteLine(value.Value);
            facade.Communication.ClosePinpadConnection("Carnaval 2017");
        }
        [Test]
        public void GetValueInOptionsShort_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();
            PinpadFacade facade = new PinpadFacade(conn);

            Nullable<short> value = facade.Keyboard.DataPicker.GetValueInOptions(
                      "Parcelas",
                      false,
                      2,
                      3,
                      4,
                      5,
                      6);

            Debug.WriteLine(value.Value);
        }
        [Test]
        public void GetValueInOptionsString_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();
            PinpadFacade facade = new PinpadFacade(conn);

            string value = facade.Keyboard.DataPicker.GetValueInOptions(
                      "Pokemon",
                      false,
                      "Bulbasaur",
                      "Charmander",
                      "Squirtle");

            Debug.WriteLine(value);
        }
        [Test]
        public void PinpadTransaction_ReadCard_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();
            PinpadFacade facade = new PinpadFacade(conn);
            TransactionType trnxType = TransactionType.Undefined;

            facade.TransactionService.EmvTable.UpdatePinpad(false);

            CardEntry card = facade.TransactionService.ReadCard(
                      TransactionType.Debit,
                0.1m,
                      out trnxType,
                      CardBrandMocker.Mock());

            facade.TransactionService.ReadPassword(
                      0.1m,
                      card.PrimaryAccountNumber,
                      CardType.Emv);

            Assert.IsNotNull(card);
        }
        [Test]
        public void PrintText_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();
            PinpadFacade facade = new PinpadFacade(conn);

            facade.Printer.AddLogo()
                      .AppendLine(PrinterAlignmentCode.Center, PrinterFontSize.Small, "Credenciadora Banco Pan")
                      .AppendLine(PrinterAlignmentCode.Center, PrinterFontSize.Small, "Via Estabelecimento")
                      .AppendLine(PrinterAlignmentCode.Center, PrinterFontSize.Medium, "MASTERCARD - DEBITO A VISTA")
                      .AppendLine(PrinterAlignmentCode.Center, PrinterFontSize.Medium, "525663******6251")
                      .AddSeparator()
                      .AppendLine(PrinterAlignmentCode.Center, PrinterFontSize.Medium, "MICROTEF TESTE")
                      .AppendLine(PrinterAlignmentCode.Left, PrinterFontSize.Small, "CNPJ: 12.345.678/0001-90")
                      .AppendLine(PrinterAlignmentCode.Left, PrinterFontSize.Small, "AID: A0000000001234 - ARQC: 12345678901234567")
                      .AppendLine(PrinterAlignmentCode.Left, PrinterFontSize.Small, facade.Infos.SerialNumber)
                      .AppendLine(PrinterAlignmentCode.Left, PrinterFontSize.Small, "StoneId: 1234567890123456")
                      .AppendLine(PrinterAlignmentCode.Left, PrinterFontSize.Medium, "12/12/2016 16:19")
                      .AddSeparator()
                      .AppendLine(PrinterAlignmentCode.Left, PrinterFontSize.Medium, "CARVALHO/CERES R")
                      .AppendLine(PrinterAlignmentCode.Left, PrinterFontSize.Big, "Valor: R$ 10,00")
                      .AppendLine()
                      .AddQrCode(PrinterAlignmentCode.Center, "1234567890123456")
                      .Print();
        }
        [Test]
        public void PrintImage_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();
            PinpadFacade facade = new PinpadFacade(conn);

            facade.Printer.AddLogo()
                          .AppendLine(PrinterAlignmentCode.Center,
                                            PrinterFontSize.Big,
                                            "Testando {0}",
                                            "Ceres")
                          .Print();
        }
        [Test]
        public void PinpadInfo_GetDukptSerialNumber_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();
            PinpadFacade facade = new PinpadFacade(conn);

            for (int i = 1; i <= 21; i++)
            {
                Debug.WriteLine("{0} |  DES | {1}", i, facade.Infos
                    .GetDukptSerialNumber(i,
                    CryptographyMode.DataEncryptionStandard));
                Debug.WriteLine("{0} | 3DES | {1}", i, facade.Infos
                    .GetDukptSerialNumber(i,
                    CryptographyMode.TripleDataEncryptionStandard));
            }
        }
        [Test]
        public void Pinpad_GetValueInOptionsShortWithCircularBehavior_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();

            PinpadCommunication comm = new PinpadCommunication(conn);
            PinpadInfos infos = new PinpadInfos(comm);
            IPinpadDisplay display = new PinpadDisplay(comm);
            PinpadKeyboard key = new PinpadKeyboard(comm, infos, display);

            key.DataPicker.GetValueInOptions("Menu", true, 1, 2, 3);
        }
        [Test]
        public void Pinpad_GetValueInOptionsStringWithCircularBehavior_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetFirst();

            PinpadCommunication comm = new PinpadCommunication(conn);
            PinpadInfos infos = new PinpadInfos(comm);
            IPinpadDisplay display = new PinpadDisplay(comm);
            PinpadKeyboard key = new PinpadKeyboard(comm, infos, display);

            string result = key.DataPicker.GetValueInOptions("Carnaval", true, "Simpatiaequaseamor", "OrquestraVoadora", "SargentoPimenta", "Carmelitas");
        }
        [Test]
        public void UpdateService_test()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetAt("192.168.1.106");

            if (conn != null)
            {
                IPinpadFacade pinpad = new PinpadFacade(conn);

                bool isLoaded = pinpad.UpdateService.Load(
                          System.IO.Path.Combine(
                              @"C:\Users\ccarvalho\Desktop\update-cmd",
                              "StonePinpadWifi(v1.1.1).zip"));

                pinpad.UpdateService.Update();
            }
        }
        //[Test]
        public void Pinpad_SetTimeoutWriteAndRead()
        {
            IPinpadConnection conn = PinpadConnectionProvider.GetAt("COM4");
            PinpadCommunication comm = new PinpadCommunication(conn);
            comm.SetTimeout(5000, 5000);

            Assert.AreEqual(conn.ReadTimeout, 5000);
            Assert.AreEqual(conn.WriteTimeout, 5000);
        }
    }
//#endif // !Release && !Debug
}
