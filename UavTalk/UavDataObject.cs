using System;
using System.IO;

namespace UavTalk
{
    public class UavDataObject
    {
        private int mObjectId;
        private Int16 mInstanceId;
        private byte mChecksum;

        public int ObjectId
        {
            get { return mObjectId; }
        }

        public Int16 InstanceId
        {
            get { return mInstanceId; }
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
    }
}

