#!/bin/sh

mono UavGen/bin/Debug/UavGen.exe --output=UavTalk/UavObjects/ `find xmls/*.xml`
