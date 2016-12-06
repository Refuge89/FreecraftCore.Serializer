﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Fasterflect;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class SubComplexTypeSerializerDecorator<TBaseType> : ITypeSerializerStrategy<TBaseType>
		where TBaseType : class, new()
	{
		/// <summary>
		/// Indicates the <see cref="TType"/> of the serializer.
		/// </summary>
		public Type SerializerType { get { return typeof(TBaseType); } }

		/// <summary>
		/// General serializer provider service.
		/// </summary>
		private IGeneralSerializerProvider serializerProviderService { get; }

		private ITypeSerializerStrategy<TBaseType> baseSerializerStrategy { get; }

		/// <summary>
		/// The lookup table that maps ints to the Type.
		/// </summary>
		private IDictionary<int, Type> keyToTypeLookup { get; }

		/// <summary>
		/// The lookup table that maps types to an int.
		/// </summary>
		private IDictionary<Type, int> typeToKeyLookup { get; }

		/// <summary>
		/// Indicates the context requirement for this serializer strategy.
		/// (Ex. If it requires context then a new one must be made or context must be provided to it for it to serializer for multiple members)
		/// </summary>
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		public SubComplexTypeSerializerDecorator(IGeneralSerializerProvider serializerProvider)
		{
			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided {nameof(serializerProvider)} service was null.");

			serializerProviderService = serializerProvider;
			typeToKeyLookup = new Dictionary<Type, int>();
			keyToTypeLookup = new Dictionary<int, Type>();

			//TODO: Support base. Right now there is no point really because base types can't have serializable fields mixed with child fields anyway
			//0 is reserved for base
			keyToTypeLookup.Add(0, typeof(TBaseType));

			foreach(WireMessageBaseTypeAttribute wa in typeof(TBaseType).Attributes<WireMessageBaseTypeAttribute>())
			{
				try
				{
					keyToTypeLookup.Add(wa.Index, wa.ChildType);
					typeToKeyLookup.Add(wa.ChildType, wa.Index);
				}
				catch(ArgumentException e)
				{
					throw new InvalidOperationException($"Failed to register child Type: {wa.ChildType} for BaseType: {typeof(TBaseType).FullName} due to likely duplicate key index for {wa.Index}. Index must be unique per Type.", e);
				}
			}
		}

		/// <summary>
		/// Perform the steps necessary to serialize this data.
		/// </summary>
		/// <param name="value">The value to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		public void Write(TBaseType value, IWireMemberWriterStrategy dest)
		{
			if (value == null)
				throw new Exception();

			if (!typeToKeyLookup.ContainsKey(value.GetType()))
				throw new InvalidOperationException($"Cannot serialize Type: {value.GetType()} in {this.GetType().FullName}.");

			dest.Write((byte)typeToKeyLookup[value.GetType()]);

			ITypeSerializerStrategy serializer = serializerProviderService.Get(value.GetType());

			if (serializer == null)
				throw new InvalidOperationException($"Couldn't locate serializer for {value.GetType().FullName} in the {nameof(IGeneralSerializerProvider)} service.");

			serializer.Write(value, dest);
		}

		/// <summary>
		/// Perform the steps necessary to deserialize this data.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>The updated / replacement value.</returns>
		public TBaseType Read(IWireMemberReaderStrategy source)
		{
			//Incoming should be a byte that indicates the child type to use
			//Read it to lookup in the map to determine which type we should create
			byte childIndexRequested = source.ReadByte();

			//Check if we have that index
			if (!keyToTypeLookup.ContainsKey(childIndexRequested))
				throw new InvalidOperationException($"{this.GetType()} attempted to deserialize to a child type with Index: {childIndexRequested} but the index didn't exist in the lookup table. Check the Type: {typeof(TBaseType).FullName} attributes for duplicate index.");

			Type childTypeRequest = keyToTypeLookup[childIndexRequested];

			if(childTypeRequest == null)
				throw new InvalidOperationException($"{this.GetType()} attempted to deserialize to a child type with Index: {childIndexRequested} but the lookup table provided a null type. This may indicate a failure in registeration of child types.");

			return serializerProviderService.Get(childTypeRequest).Read(source) as TBaseType;
		}

		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((TBaseType)value, dest);
		}

		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}
	}

/*//First all children must be registered. Their object graphs may contain members that are complex
				IEnumerable<WireMessageBaseTypeAttribute> subtypeAttributes = typeBeingSerialized.GetCustomAttributes<WireMessageBaseTypeAttribute>(false);

				foreach (Type childType in subtypeAttributes.Select(x => x.ChildType))
				{
					//Call generic registry
					contractRegistry.CallMethod(new Type[] { childType }, "RegisterType"); //todo: replace reflection
				}

				//At this point all children are registered. A SubComplexTypeDecorator would be able to gather and map the serializers
				ITypeSerializerStrategy generatedSerializer = typeof(SubComplexTypeSerializerDecorator<>).MakeGenericType(typeBeingSerialized).CreateInstance(serializerFactoryService) as ITypeSerializerStrategy;

				if (generatedSerializer == null)
					throw new InvalidOperationException($"Failed to generate decorated serializer for Type: {typeBeingSerialized}.");

				return generatedSerializer;*/
}
