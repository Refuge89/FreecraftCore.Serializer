﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class CompressionTests
	{
		[Test]
		public static void Test_Can_Register_Compression_Marked_Class()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestInt32ArrayCompression>();
			serializer.Compile();
		}

		[Test]
		public static void Test_Can_Serialize_Compression_Marked_Class()
		{
			//arrange
			int[] values = new int[] { 1, 5, 7, 1, 1, 1, 1, 1, 1, 5, 1, 5, 6 };
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestInt32ArrayCompression>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestInt32ArrayCompression(values));

			//assert
			Assert.NotNull(bytes);
			Assert.IsTrue(bytes.Length < (values.Length * sizeof(int)));
		}

		[Test]
		public static void Test_Can_Deserialize_Compression_Marked_Class()
		{
			//arrange
			int[] values = new int[] { 1, 5, 7, 1, 1, 1, 1, 1, 1, 5, 1, 5, 6 };
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestInt32ArrayCompression>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestInt32ArrayCompression(values));
			TestInt32ArrayCompression obj = serializer.Deserialize<TestInt32ArrayCompression>(bytes);

			//assert
			Assert.NotNull(obj);
			Assert.NotNull(obj.Integers);
			Assert.AreEqual(values.Length, obj.Integers.Length);
			for(int i = 0; i < values.Length; i++)
				Assert.AreEqual(values[i], obj.Integers[i]);
		}

		[WireDataContract]
		public class TestInt32ArrayCompression
		{
			[Compress]
			[WireMember(1)]
			public int[] Integers;
			
			public TestInt32ArrayCompression(int[] integers)
			{
				if (integers == null) throw new ArgumentNullException(nameof(integers));

				Integers = integers;
			}

			public TestInt32ArrayCompression()
			{
				
			}
		}
	}
}