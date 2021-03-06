# FreecraftCore.Serializer
### *Now with Netstandard1.5 and Async/Await support!*
FreecraftCore is an open-source C#/.NET World of Warcraft emulation project. It is derived from the reverse engineering work done by Mangos and Trinitycore.

The FreecraftCore.Payload.Serializer is a metadata based serializer for WoW packets based on [JAM](https://www.youtube.com/watch?v=hCsEHYwjqVE) and inspired by the design of [Protobuf-net](https://github.com/mgravell/protobuf-net) by Marc.

## Implementation

The Serializer reflects on Types for attributes (looks at metadata) marked on wire message types to build a simple serializer that reads and writes from a stream. The primitive datatype reading and writing is based on Trinitycore's [ByteBuffer](https://github.com/TrinityCore/TrinityCore/blob/3.3.5/src/server/shared/Packets/ByteBuffer.h). All serialization is just a decoration of the primitive datatype serialization strategies. 

In short, FreecraftCore.Serializer knows how to serialize primitives in the fashion that the WoW protocol excepts and all serialization functionality is an extension of this via decoration.

To understand how this serializer works read documentation related to Protobuf-Net [here](https://www.codeproject.com/articles/642677/protobuf-net-the-unofficial-manual). The design is similar.

## Creating a Payload/WireMessage/Packet

```
[WireDataContract] //mark wire messages with this attribute
public class MyMessagePayload
{
  [WireMember(1)] //Mark members you want sent over the network with [WireMember] attributes. A unique per class int key is required.
  public int Damage;
 
  [WireMember(5)] //the keys don't need to go in sequence. No benefit really.
  public string TargetName;
  
  [WireMember(7)]
  public MyCustomType Instance; //you can send custom types over the network too.
  
  //Arrays, Enums and even some basic Polymorphism works!
  
  public MyMessagePayload() //as of right now the serializer requires a parameterless ctor. This will be changed in the future
  {
  
  }
}

public class MyCustomType //non wiremessages don't need to be marked but if you want to use polymorphic serialization you should mark
{
  [WireMember(1)]
  public int data;
  
  public MyMessagePayload() //non wire messages still require a parameterless ctor (for now)
  {
  
  }
}
```

## Unsupported Serialization Scenarios

```
[KnownSize(5)]
[WireMember(1)]
SomeType[][] //Multidimensional arrays are partially supported but not fixed sized ones.
```

```
[WireMember(1)]
IEnumerable<SomeType> //Any collection except arrays are currently unsupported but planned for future support
```
### Polymorphic Serialization is now supported for classes! Interfaces in the future.

## Builds

Available on a Nuget Feed: https://www.myget.org/F/freecraftcore/api/v2 [![freecraftcore MyGet Build Status](https://www.myget.org/BuildSource/Badge/freecraftcore?identifier=c9b72800-10c2-49c7-a779-feda4e1da5af)](https://www.myget.org/)

## Tests

|    | Linux Debug | Windows .NET Debug |
|:---|----------------:|------------------:|
|**master**| [![Build Status](https://travis-ci.org/FreecraftCore/FreecraftCore.Serializer.svg?branch=master)](https://travis-ci.org/FreecraftCore/FreecraftCore.Serializer) | TODO |
|**dev**| [![Build Status](https://travis-ci.org/FreecraftCore/FreecraftCore.Serializer.svg?branch=dev)](https://travis-ci.org/FreecraftCore/FreecraftCore.Serializer) | TODO |

#### Windows - Unit Tests

(Done locally)

## Licensing

This project is licensed under the GNU AFFERO GENERAL PUBLIC LICENSE so follow it or I will report you to the software [gods](https://www.gnu.org/licenses/gpl-violation.en.html).
