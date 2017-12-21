﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Strategy that reads ALL bytes in the source and writes all bytes to the destination.
	/// This is like the default fallback for an unmarked byte array. It is the only supported array type
	/// that doesn't have known or sendsize semnatics (as of right now).
	/// </summary>
	public sealed class DefaultByteArraySerializerStrategy : SimpleTypeSerializerStrategy<byte[]>
	{
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		public override byte[] Read(IWireStreamReaderStrategy source)
		{
			return source.ReadAllBytes();
		}

		public override async Task<byte[]> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			return await source.ReadAllBytesAsync()
				.ConfigureAwait(false);
		}

		public override void Write(byte[] value, IWireStreamWriterStrategy dest)
		{
			dest.Write(value);
		}

		public override async Task WriteAsync(byte[] value, IWireStreamWriterStrategyAsync dest)
		{
			await dest.WriteAsync(value)
				.ConfigureAwait(false);
		}
	}
}
