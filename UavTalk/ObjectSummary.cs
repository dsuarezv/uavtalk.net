using System;
using System.Collections.Generic;

namespace UavTalk
{
    public partial class ObjectSummary
    {
        public static Dictionary<UInt32, Type> GetObjectIds()
        {
            return mObjectIds;
        }

        private static UInt32 RegisterObjectType(UInt32 id, Type type)
        {
            mObjectIds[id] = type;

            return id;
        }

        private static Dictionary<UInt32, Type> mObjectIds = new Dictionary<uint, Type>();
    }
}

