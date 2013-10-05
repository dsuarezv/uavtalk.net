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

            if (!mObjectIds.TryGetValue(id, out t))
                throw new Exception(string.Format("Object Id 0x{0:x8} not found", id));

            ConstructorInfo ci = t.GetConstructor(new Type[] { });
            return ci.Invoke(new object[] {}) as UavDataObject;
        }


        protected static UInt32 RegisterObjectType(UInt32 id, Type type)
        {
            mObjectIds[id] = type;

            return id;
        }

        protected void NotifyUpdated()
        {

        }

        protected virtual void DeserializeBody(BinaryReader stream, UavDataObject target)
        {

        }

        protected virtual void SerializeBody(BinaryWriter stream)
        {

        }

        private static Dictionary<UInt32, Type> mObjectIds = new Dictionary<uint, Type>();
    }
}

