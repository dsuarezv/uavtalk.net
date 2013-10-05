using System;
using System.IO;
using System.Collections.Generic;

namespace UavTalk
{
    public class UavDataObject
    {
        public UInt32 ObjectId;
        public UInt16 InstanceId;
        public byte Checksum;
        public bool IsSingleInstance = true;

        public static Dictionary<UInt32, Type> GetObjectIds()
        {
            return ObjectSummary.GetObjectIds();
        }

        protected void NotifyUpdated()
        {

        }

        internal virtual void DeserializeBody(BinaryReader stream)
        {

        }

        internal virtual void SerializeBody(BinaryWriter stream)
        {

        }

    }
}

