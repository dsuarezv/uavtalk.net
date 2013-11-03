using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UavTalk;

namespace UavTalkParser
{
    public delegate void SerialTelemetryMessage(UavDataObject obj);


    public class SerialTelemetryParser
    {
        public event SerialTelemetryMessage OnTelemetryMessage;


        public static string[] GetComPorts()
        {
            return SerialPort.GetPortNames();
        }

        public SerialTelemetryParser(string comPort)
        {
            mComPort = comPort;
        }

        public void Loop()
        {
            using (mPort = new SerialPort(mComPort, 57600, Parity.None, 8, StopBits.One  ))
            {
                mPort.DataReceived += HandleDataReceived;
                mPort.Open();

                using (mReader = new BinaryReader(mPort.BaseStream))
                {
                    Process(mReader);
                }


                mPort.Close();
            }
        }



        public void AsyncLoop()
        { 
            
        }


        // __ Impl ____________________________________________________________
       

        private void Process(BinaryReader reader)
        {
            UavTalkWalker w = new UavTalkWalker();

            while (true)
            {
                try
                {
                    UavDataObject obj = w.GetNextObject(reader);

                    if (OnTelemetryMessage != null) 
                        OnTelemetryMessage(obj);
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

        private void HandleDataReceived (object sender, SerialDataReceivedEventArgs e)
        {
            return;
        }


        private BinaryReader mReader;
        private SerialPort mPort;
        private string mComPort;
    }
}
