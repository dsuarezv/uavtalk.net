#!/bin/sh

rm UavTalk/UavObjects/*.cs
echo Generating UavObjects...
mono UavGen/bin/Debug/UavGen.exe --output=UavTalk/UavObjects/ `find xmls/*.xml`