#!/bin/sh

rm UavTalk/UavObjects/*.cs

mono UavGen/bin/Debug/UavGen.exe --output=UavTalk/UavObjects/ `find xmls/*.xml`
