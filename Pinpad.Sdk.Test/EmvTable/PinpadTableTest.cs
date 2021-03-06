﻿using System;
using Microtef.CrossPlatform;
using Pinpad.Sdk.Model;
using Pinpad.Sdk.Commands;
using NUnit.Framework;
using Microtef.CrossPlatform.TypeCode;
using Moq;

namespace Pinpad.Sdk.Test.EmvTable
{
    [TestFixture]
    public class PinpadTableTest
    {
        IPinpadFacade PinpadFacadeMock;
        IPinpadConnection ConnectionStub;

        #region setting stuff up
        [SetUp]
        public void Setup()
        {
            this.PinpadFacadeMock = Mock.Of<IPinpadFacade>();
            this.ConnectionStub = new Stubs.PinpadConnectionStub();
        }
        public CapkTable GetCapk(int i)
        {
			CapkTable capk = new CapkTable();
			switch (i)
			{
				case 4:
					capk.T2_RID.Value = new HexadecimalData("A000000004");
					capk.T2_CAPKIDX.Value = new HexadecimalData("04");
					capk.T2_EXP.Value = new HexadecimalData("030000");
					capk.T2_MOD.Value = new HexadecimalData("A6DA428387A502D7DDFB7A74D3F412BE762627197B25435B7A81716A700157DDD06F7CC99D6CA28C2470527E2C03616B9C59217357C2674F583B3BA5C7DCF2838692D023E3562420B4615C439CA97C44DC9A249CFCE7B3BFB22F68228C3AF13329AA4A613CF8DD853502373D62E49AB256D2BC17120E54AEDCED6D96A4287ACC5C04677D4A5A320DB8BEE2F775E5FEC50000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
					return capk;
				case 5:
					capk.T2_RID.Value = new HexadecimalData("A000000004");
					capk.T2_CAPKIDX.Value = new HexadecimalData("05");
					capk.T2_EXP.Value = new HexadecimalData("030000");
					capk.T2_MOD.Value = new HexadecimalData("B8048ABC30C90D976336543E3FD7091C8FE4800DF820ED55E7E94813ED00555B573FECA3D84AF6131A651D66CFF4284FB13B635EDD0EE40176D8BF04B7FD1C7BACF9AC7327DFAA8AA72D10DB3B8E70B2DDD811CB4196525EA386ACC33C0D9D4575916469C4E4F53E8E1C912CC618CB22DDE7C3568E90022E6BBA770202E4522A2DD623D180E215BD1D1507FE3DC90CA310D27B3EFCCD8F83DE3052CAD1E48938C68D095AAC91B5F37E28BB49EC7ED597000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
					return capk;
				case 6:
					capk.T2_RID.Value = new HexadecimalData("A000000004");
					capk.T2_CAPKIDX.Value = new HexadecimalData("06");
					capk.T2_EXP.Value = new HexadecimalData("030000");
					capk.T2_MOD.Value = new HexadecimalData("CB26FC830B43785B2BCE37C81ED334622F9622F4C89AAE641046B2353433883F307FB7C974162DA72F7A4EC75D9D657336865B8D3023D3D645667625C9A07A6B7A137CF0C64198AE38FC238006FB2603F41F4F3BB9DA1347270F2F5D8C606E420958C5F7D50A71DE30142F70DE468889B5E3A08695B938A50FC980393A9CBCE44AD2D64F630BB33AD3F5F5FD495D31F37818C1D94071342E07F1BEC2194F6035BA5DED3936500EB82DFDA6E8AFB655B1EF3D0D7EBF86B66DD9F29F6B1D324FE8B26CE38AB2013DD13F611E7A594D675C4432350EA244CC34F3873CBA06592987A1D7E852ADC22EF5A2EE28132031E48F74037E3B34AB747F");
					return capk;
				case 7:
					capk.T2_RID.Value = new HexadecimalData("A000000003");
					capk.T2_CAPKIDX.Value = new HexadecimalData("07");
					capk.T2_EXP.Value = new HexadecimalData("030000");
					capk.T2_MOD.Value = new HexadecimalData("A89F25A56FA6DA258C8CA8B40427D927B4A1EB4D7EA326BBB12F97DED70AE5E4480FC9C5E8A972177110A1CC318D06D2F8F5C4844AC5FA79A4DC470BB11ED635699C17081B90F1B984F12E92C1C529276D8AF8EC7F28492097D8CD5BECEA16FE4088F6CFAB4A1B42328A1B996F9278B0B7E3311CA5EF856C2F888474B83612A82E4E00D0CD4069A6783140433D50725F0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
					return capk;
				case 8:
					capk.T2_RID.Value = new HexadecimalData("A000000003");
					capk.T2_CAPKIDX.Value = new HexadecimalData("08");
					capk.T2_EXP.Value = new HexadecimalData("030000");
					capk.T2_MOD.Value = new HexadecimalData("D9FD6ED75D51D0E30664BD157023EAA1FFA871E4DA65672B863D255E81E137A51DE4F72BCC9E44ACE12127F87E263D3AF9DD9CF35CA4A7B01E907000BA85D24954C2FCA3074825DDD4C0C8F186CB020F683E02F2DEAD3969133F06F7845166ACEB57CA0FC2603445469811D293BFEFBAFAB57631B3DD91E796BF850A25012F1AE38F05AA5C4D6D03B1DC2E568612785938BBC9B3CD3A910C1DA55A5A9218ACE0F7A21287752682F15832A678D6E1ED0B000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
					return capk;
				case 9:
					capk.T2_RID.Value = new HexadecimalData("A000000003");
					capk.T2_CAPKIDX.Value = new HexadecimalData("09");
					capk.T2_EXP.Value = new HexadecimalData("030000");
					capk.T2_MOD.Value = new HexadecimalData("9D912248DE0A4E39C1A7DDE3F6D2588992C1A4095AFBD1824D1BA74847F2BC4926D2EFD904B4B54954CD189A54C5D1179654F8F9B0D2AB5F0357EB642FEDA95D3912C6576945FAB897E7062CAA44A4AA06B8FE6E3DBA18AF6AE3738E30429EE9BE03427C9D64F695FA8CAB4BFE376853EA34AD1D76BFCAD15908C077FFE6DC5521ECEF5D278A96E26F57359FFAEDA19434B937F1AD999DC5C41EB11935B44C18100E857F431A4A5A6BB65114F174C2D7B59FDF237D6BB1DD0916E644D709DED56481477C75D95CDD68254615F7740EC07F330AC5D67BCD75BF23D28A140826C026DBDE971A37CD3EF9B8DF644AC385010501EFC6509D7A41");
					return capk;
			}

			return null;
        }
        #endregion

        [Test]
        public void PinpadTable_Construction_ShouldThrowException_IfPinpadCommunicationIsNull ()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Arrange
                IPinpadCommunication nullPinpadCommunication = null;

                // Act
                PinpadTable table = new PinpadTable(nullPinpadCommunication);
            });
        }
        [Test]
        public void PinpadTable_Construction_ShouldNotReturnNull_IfParametersAreCorrect ()
        {
            // Arrange
            IPinpadConnection conn = Mock.Of<IPinpadConnection>();
            Stubs.PinpadCommunicationStub pinpadCommunicationStub = 
                new Stubs.PinpadCommunicationStub();

            // Act
            PinpadTable table = new PinpadTable(pinpadCommunicationStub);

            // Assert
            Assert.IsNotNull(table);
        }
        [Test]
        public void PinpadTable_AddEntry_ShouldThrowException_IfUnknownTableEntry ()
        {
            // Assert
            Assert.Throws<NotImplementedException>(() =>
            {
                // Arrange
                Stubs.PinpadCommunicationStub pinpadCommunicationStub =
                    new Stubs.PinpadCommunicationStub();
                Dummies.DummyTable dummyTable = new Dummies.DummyTable();
                PinpadTable table = new PinpadTable(pinpadCommunicationStub);
				
                // Act
                table.AddEntry(dummyTable);
            });
        }
        [Test]
        public void PinpadTable_CapkTable_ShouldMatchTheNumberOrTablesAdded ()
        {
            // Arrange
            PinpadTable table = new PinpadTable(new Stubs.PinpadCommunicationStub());

            // Assert
            Assert.AreEqual(table.CapkTable.Count, 0);

            // Arrange
            for (int i = 0; i < 10; i++) { table.AddEntry(this.GetCapk(4)); }

            // Assert
            Assert.AreEqual(table.CapkTable.Count, 1);
        }
    }
}
