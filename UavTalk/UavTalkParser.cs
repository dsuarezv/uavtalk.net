using System;
using System.IO;

namespace UavTalk
{
    public enum UavTalk_MessageType { OBJ = 0x20, OBJ_REQ = 0x21, OBJ_ACK = 0x22, ACK = 0x23, NACK = 0x24 };



    public class UavTalkParser
    {
        public UavTalkParser()
        {

        }

        private void SyncStream()
        {
            // Search for a 0x3C + messagetype header
        }

        private UavDataObject DeserializeHeader(BinaryReader stream)
        {
            // Identifies the type of object and creates the proper object for further processing

            return null;
        }

        protected virtual void DeserializeBody(BinaryReader stream, UavDataObject target)
        {

        }
    }
}

