﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Size strategy for dynamically sized collections.
	/// </summary>
	public class ByteSizeCollectionSizeStrategy : ICollectionSizeStrategy
	{
		//TODO: Should use the byte strategy not direct read/write

		/// <summary>
		/// Determines the size of the collection from the stream.
		/// </summary>
		public int Size(IWireStreamReaderStrategy reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			//Read the byte size from the stream.
			return reader.ReadByte();
		}

		/// <summary>
		/// The size to consider the collection.
		/// </summary>
		public int Size<TCollectionType, TElementType>(TCollectionType collection, IWireStreamWriterStrategy writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			//Since the size is unknown it's critical that we write the size to the stream.
			writer.Write((byte)collection.Count());

			//We don't know the size so just provide the size of the collection
			return collection.Count();
		}

		/// <inheritdoc />
		public async Task<int> SizeAsync<TCollectionType, TElementType>(TCollectionType collection, IWireStreamWriterStrategyAsync writer) where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			byte count = (byte) collection.Count();

			//yield until we write (shouldn't take long and maybe syncronous is more efficient and performant but async API should be fully async) 
			await writer.WriteAsync(count);

			//We don't know the size so just provide the size of the collection
			return count;
		}

		/// <inheritdoc />
		public async Task<int> SizeAsync(IWireStreamReaderStrategyAsync reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			//Read the byte size from the stream.
			return await reader.ReadByteAsync();
		}
	}
}
