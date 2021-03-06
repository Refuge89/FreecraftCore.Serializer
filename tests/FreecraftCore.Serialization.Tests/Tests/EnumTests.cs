﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class EnumTests
	{
		[Test]
		public static void Test_Serializer_Can_Register_WireType_With_Enum_Field()
		{
			//arrange
			SerializerService service = new SerializerService();

			//act
			Assert.DoesNotThrow(() => service.RegisterType<WireDataContractWithEnum>());
			Assert.True(service.isTypeRegistered<WireDataContractWithEnum>());
		}

		[Test]
		public static void Test_Serializer_Can_Read_Write_Type_With_Enum_Field()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<WireDataContractWithEnum>();
			service.Compile();

			//act
			WireDataContractWithEnum testInstance = service.Deserialize<WireDataContractWithEnum>((service.Serialize<WireDataContractWithEnum>(new WireDataContractWithEnum(TestEnum.Ok))));

			//assert
			Assert.AreEqual(testInstance.test, TestEnum.Ok);
		}

		[Test]
		public static void Test_Serializer_Can_Register_WireType_With_EnumString_Field()
		{
			//arrange
			SerializerService service = new SerializerService();

			//act
			Assert.DoesNotThrow(() => service.RegisterType<WireDataContractWithStringEnum>());
			Assert.True(service.isTypeRegistered<WireDataContractWithStringEnum>());
		}

		[Test]
		public static void Test_Serializer_Can_Read_Write_Type_With_EnumString_Field()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<WireDataContractWithStringEnum>();
			service.Compile();

			//act
			WireDataContractWithStringEnum testInstance = service.Deserialize<WireDataContractWithStringEnum>((service.Serialize<WireDataContractWithStringEnum>(new WireDataContractWithStringEnum(TestEnum.Something))));

			//assert
			Assert.AreEqual(testInstance.test, TestEnum.Something);
		}

		[Test]
		public static void Test_Serializer_Can_Read_Enum_As_String()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<WireDataContractWithStringEnum>();
			service.RegisterType<TestIsSerializingToStringClass>();
			service.Compile();

			//act
			byte[] bytes = service.Serialize<WireDataContractWithStringEnum>(new WireDataContractWithStringEnum(TestEnum.Something))
				.Concat(new byte[1] {0})
				.ToArray();

			Assert.True(bytes.Length == 10, "Expected length of 9 from KnownSize string. Appended nullterminator for test");
			TestIsSerializingToStringClass testInstance = service.Deserialize<TestIsSerializingToStringClass>(bytes);

			//assert
			Assert.AreEqual(testInstance.test, TestEnum.Something.ToString());
		}

		[Test]
		public static void Test_Serializer_Doesnt_Throw_On_EnumString_Serializer_Building_When_A_String_Serializer_Is_Avaliable()
		{
			//arrange
			SerializerService service = new SerializerService();

			service.RegisterType<ClassWithStringToMakeStringSerializerAvailable>();
			Assert.Throws<Exception>(() => service.RegisterType<TestEnumStringFault>()); //this was causing problems when you tried to register multiple enumstrings due to fault with key lookup
		}

		[Test]
		public static void Test_EnumString_Produces_Expected_Byte_Size()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<KnownSizeEnumStringTest>();
			service.Compile();

			//act
			byte[] bytes = service.Serialize(new KnownSizeEnumStringTest(TestStringEnum.Hello));

			//using KnownSize won't null terminate extend
			Assert.AreEqual(5, bytes.Length);
			Assert.True(bytes[bytes.Length - 1] != 0);
			Assert.True(bytes[bytes.Length - 2] != 0);
		}

		[Test]
		public static void Test_EnumStringUTF16_Produces_Expected_Byte_Size()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<KnownSizeEnumStringTestUTF16>();
			service.Compile();

			//act
			byte[] bytes = service.Serialize(new KnownSizeEnumStringTestUTF16(TestStringEnum.Hello));

			//using KnownSize won't null terminate extend
			Assert.AreEqual(5 * 2, bytes.Length);
			Assert.AreEqual(TestStringEnum.Hello.ToString(), Encoding.Unicode.GetString(bytes));
		}

		[WireDataContract]
		public class KnownSizeEnumStringTestUTF16
		{
			[Encoding(EncodingType.UTF16)]
			[KnownSize(5)]
			[EnumString]
			[WireMember(1)]
			public TestStringEnum test;

			public KnownSizeEnumStringTestUTF16(TestStringEnum t)
			{
				test = t;
			}

			public KnownSizeEnumStringTestUTF16()
			{

			}
		}

		[WireDataContract]
		public class KnownSizeEnumStringTest
		{
			[KnownSize(5)]
			[EnumString]
			[WireMember(1)]
			public TestStringEnum test;

			public KnownSizeEnumStringTest(TestStringEnum t)
			{
				test = t;
			}

			public KnownSizeEnumStringTest()
			{

			}
		}

		public enum TestStringEnum
		{
			Hello,
			Morel,
			Mabel,
		}

		[WireDataContract]
		public class ClassWithStringToMakeStringSerializerAvailable
		{
			[KnownSize(3)]
			[WireMember(1)]
			public string value;
		}

		[WireDataContract]
		public class TestEnumStringFault
		{
			[EnumString]
			[KnownSize(3)]
			[WireMember(1)]
			public TestEnum value;

			[EnumString]
			[KnownSize(3)]
			[WireMember(1)]
			public TestEnum2 value2;

			public TestEnumStringFault()
			{

			}
		}

		[WireDataContract]
		public class WireDataContractWithEnum
		{
			[WireMember(1)]
			public TestEnum test;

			public WireDataContractWithEnum(TestEnum value)
			{
				test = value;
			}

			public WireDataContractWithEnum()
			{

			}
		}

		[WireDataContract]
		public class WireDataContractWithStringEnum
		{
			[KnownSize(9)]
			[EnumString]
			[WireMember(1)]
			public TestEnum test;

			public WireDataContractWithStringEnum(TestEnum value)
			{
				test = value;
			}

			public WireDataContractWithStringEnum()
			{

			}
		}

		[WireDataContract]
		public class TestIsSerializingToStringClass
		{
			[WireMember(1)]
			public string test;

			public TestIsSerializingToStringClass()
			{

			}
		}

		public enum TestEnum : byte
		{
			Ok,
			No,
			Something
		}
		
		public enum TestEnum2 : byte
		{

		}
	}
}
