using System;
using System.IO;

namespace UavTalk
{
    public class UavDataObject
    {
        public UInt32 ObjectId;
        public Int16 InstanceId;
        public byte Checksum;


        protected void NotifyUpdated()
        {

        }

        protected virtual void DeserializeBody(BinaryReader stream, UavDataObject target)
        {

        }

        protected virtual void SerializeBody(BinaryWriter stream)
        {

        }
    }
}

