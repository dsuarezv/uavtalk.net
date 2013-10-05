using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace UavTalk
{
    public class UavDataObject
    {
        public UInt32 ObjectId;
        public Int16 InstanceId;
        public byte Checksum;


        public static UavDataObject CreateObject(UInt32 id)
        {
            Type t; 

            if (!GetObjectIds().TryGetValue(id, out t))
                throw new Exception(string.Format("Object Id 0x{0:x8} not found", id));

            ConstructorInfo ci = t.GetConstructor(new Type[] { });
            return ci.Invoke(new object[] {}) as UavDataObject;
        }

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

