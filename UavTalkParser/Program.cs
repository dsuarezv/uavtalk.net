using System;
using System.IO;
using System.Collections.Generic;
using UavTalk;

namespace UavTalkParser
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage();
                return;
            }

            ObjectSummary.RegisterObjects();

            try
            {
                switch (args[0])
                {
                    case "printids":
                        PrintIds();
                        return;
                    case "dumplog": 
                        if (args.Length == 2)
                        {
                            DumpLog(args[1]);
                            return;
                        }
                        break;
                    case "listcomports":
                        ListComPorts();
                        return;
                    case "readtelemetry":
                        if (args.Length == 2)
                        {
                            ReadTelemetry(args[1]);
                            return;
                        }
                        break;
                }
                 
                PrintUsage();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR: {0}", ex.Message);
            }
        }

        private static void PrintUsage()
        {
            P("Usage: UavTalkParser <command> <command-arguments>");
            P("  Available commands:");
            P("    printids");
            P("        Prints registered object ids and associated type.");
            P("    dumplog <logfile>");
            P("        Parses given logfile and prints info on found objects.");
            P("    listcomports");
            P("        Lists the available COM ports on this computer");
            P("    readtelemetry <comport>");
            P("        Connects with an openpilot board on the given COM port and prints telemetry.");
            P("");
        }

        private static void P(string msg, params object[] args)
        {
            Console.WriteLine(msg, args);
        }

        private static void PrintIds()
        {
            foreach (KeyValuePair<UInt32, Type> entry in UavDataObject.GetObjectIds())
            {
                P("0x{0:x8} -> {1}", entry.Key, entry.Value);
            }
        }

        private static void DumpLog(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    new LogDumper(reader).Process();
                }
            }
        }

        private static void ListComPorts()
        {
            foreach (string s in SerialTelemetryParser.GetComPorts())
            {
                P(s);
            }
        }

        private static void ReadTelemetry(string comPort)
        {
            SerialTelemetryParser p = new SerialTelemetryParser(comPort);
            p.OnTelemetryMessage += OnSerialTelemetryMessage;
            p.Loop();
        }

        static void OnSerialTelemetryMessage(UavDataObject obj)
        {
            Console.WriteLine(obj.ToString());
        }
    }
}
