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

        public UavDataObject GetNextObject(BinaryReader stream)
        {
            while ( SyncStream(stream) != UavTalk_MessageType.OBJ)
            {
                // Skip ACKs and other stuff for now. Only objects with data.
            }

            UavDataObject result = DeserializeHeader(stream);

            DeserializeBody(stream, result);

            return result;
        }

        private UavTalk_MessageType SyncStream(BinaryReader stream)
        {
            byte messageType;

            while (true)
            {
                // Search for a 0x3C
                while (stream.ReadByte() != 0x3C)
                {
                    // Skip bytes until a packet start begins
                }

                messageType = stream.ReadByte();

                if (messageType >= (byte)UavTalk_MessageType.OBJ 
                 && messageType <= (byte)UavTalk_MessageType.NACK)
                    break;
            }

            return (UavTalk_MessageType)messageType;
        }

        private UavDataObject DeserializeHeader(BinaryReader stream)
        {
            // Identifies the type of object and creates the proper object for further processing

            Int16 lenght = stream.ReadInt16();
            UInt32 objId = stream.ReadUInt32();
            UavDataObject result = UavDataObject.CreateObject(objId);



            UInt16 instanceId = stream.ReadUInt16();

            return null;
        }

        protected void DeserializeBody(BinaryReader stream, UavDataObject target)
        {
            target.DeserializeBody(stream);
        }
    }
}

