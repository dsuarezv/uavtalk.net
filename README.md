# UAVTalk.NET


This is a UAVTalk class generator for C#. UAVTalk is the communication protocol used by OpenPilot to send commands and telemetry from a UAV that uses CC3D or Revolution autopilots to the OpenPilot Ground Control Station. More details of the protocol here: http://wiki.openpilot.org/display/Doc/UAVTalk

## UavObjectGenerator


This is the generator library. It takes XML UAVTalk protocol definition files as input and generates C# class files with serialization methods, as well as a summary class that acts a UavObject factory. 

As of this writing, the protocol definitions are located here: <https://github.com/openpilot/OpenPilot/tree/master/shared/uavobjectdefinition>

## UavGen

A sample command line utility that uses the UavObjectGenerator library. Run it without arguments to get usage instructions. 

## UavTalk

This library contains the generated classes and some support code to ease UavObject instantiation. It also includes a UavTalkWalker class that parses the protocol from the wire. 

## UavTalkParser

This is a sample command line utility that uses the UavTalk library. It can take a logfile from the OpenPilot ground control station software and dump it on screen with details of every packet. Make sure to generate the UavObject classes for the same version of the GCS software that you are using. 