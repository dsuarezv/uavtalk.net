using System;
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
            }
             
            PrintUsage();
        }

        private static void PrintUsage()
        {
            P("Usage: UavTalkParser <command> <command-arguments>");
            P("  Available commands:");
            P("    printids");
            P("        Prints registered object ids and associated type.");
            P("    dumplog <logfile>");
            P("        Parses given logfile and prints info on found objects.");
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
                P("{0:x8} -> {1}", entry.Key, entry.Value);
            }
        }

        private static void DumpLog(string filename)
        {

        }
    }
}
