using System;
using System.IO;
using UavTalk;

namespace UavTalkParser
{
    public class LogDumper
    {
        private BinaryReader mInput;

        public LogDumper(BinaryReader stream)
        {
            mInput = stream;
        }

        public void Process()
        {
            UavTalkWalker w = new UavTalkWalker();

            while (true)
            {
                try
                {
                    UavDataObject obj = w.GetNextObject(mInput);
                    Console.WriteLine(obj.ToString());
                }
                catch (EndOfStreamException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                }
            }
        }
    }
}

