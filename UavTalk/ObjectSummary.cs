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

        private static void RegisterObjectType(UInt32 id, Type type)
        {
            mObjectIds[id] = type;
        }

        private static Dictionary<UInt32, Type> mObjectIds = new Dictionary<uint, Type>();
    }
}

