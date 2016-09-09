﻿using System;

namespace FreeCore.Payload.Serializer
{
	/// <summary>
	/// Description of ITypeSerializer.
	/// </summary>
	public interface ITypeSerializer<TType>
	{
		/// <summary>
		/// Serializes the <typeparamref="TType">The Type this serializer can serialize.</typeparamref>
		/// </summary>
		/// <param name="toSerialize">The value/instance to serialize.</param>
		/// <returns>Array of bytes representation of the serialized value/instance.</returns>
		byte[] Serialize(TType toSerialize);
		
		/// <summary>
		/// Deserializes the byte array to a <typeparamref="TType">The Type this serializer can deserialize.</typeparamref>
		/// instance/value.
		/// </summary>
		/// <param name="toDeserialize">The value/instance to deserialize.</param>
		/// <returns>An instance/value of the Type.</returns>
		TType Deserialize(byte[] toDeserialize);
	}
}
