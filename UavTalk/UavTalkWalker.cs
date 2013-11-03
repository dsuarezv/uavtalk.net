using System;
using System.IO;

namespace UavTalk
{
    public enum UavTalk_MessageType { OBJ = 0x20, OBJ_REQ = 0x21, OBJ_ACK = 0x22, ACK = 0x23, NACK = 0x24 };



    public class UavTalkWalker
    {
        public UavTalkWalker()
        {

        }

        public UavDataObject GetNextObject(BinaryReader stream)
        {
            while (true)
            {
                UavTalk_MessageType mt = SyncStream(stream);

                switch (mt)
                {
                    case UavTalk_MessageType.OBJ:
                        UavDataObject result = DeserializeHeader(stream);
                        DeserializeBody(stream, result);
                        return result;
                    case UavTalk_MessageType.OBJ_REQ:
                    case UavTalk_MessageType.ACK:
                    case UavTalk_MessageType.NACK:
                    case UavTalk_MessageType.OBJ_ACK:
                        Console.WriteLine("Walker: " + mt.ToString());
                        break;
                }
            }
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

            stream.ReadInt16(); // length
            UInt32 objId = stream.ReadUInt32();
            UavDataObject result = ObjectSummary.CreateObject(objId);

            if (result == null)
            {
                // DAVE: add better handling: read length field and skip this packet, instead of stopping with an exception
                throw new Exception(string.Format("Unexpected ID: 0x{0:x8} at {1}", objId, stream.BaseStream.Position));
            }

            result.InstanceId = (result.IsSingleInstance) ? (UInt16)0 : stream.ReadUInt16();

            return result;
        }

        protected void DeserializeBody(BinaryReader stream, UavDataObject target)
        {
            target.DeserializeBody(stream);
        }
    }
}

