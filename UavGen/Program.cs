using System;
using System.IO;
using System.Xml;
using UavObjectGenerator;

namespace UavGen
{
    class MainClass
    {
        public static int Main(string[] args)
        {
            //XmlParser p = new XmlParser("/Users/dave/develop/Taulabs-dyquo/xmls/accels.xml");
            //XmlParser p = new XmlParser("/Users/dave/develop/Taulabs-dyquo/xmls/brushlessgimbalsettings.xml");
            //p.Generate("/Users/dave/develop/Taulabs-dyquo/output/accels.cs");

            //XmlParser.Generate(new XmlTextReader("/Users/dave/develop/Taulabs-dyquo/xmls/adcrouting.xml"), Console.Out);
            //XmlParser.Generate(new XmlTextReader("/Users/dave/develop/Taulabs-dyquo/xmls/accels.xml"), Console.Out);
            //XmlParser.Generate(new XmlTextReader("/Users/dave/develop/Taulabs-dyquo/xmls/fixedwingpathfollowersettingscc.xml"), Console.Out);

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: UavGen <list of xml definition files>");
                return 1;
            }

            foreach (string s in args)
            {
                try
                {
                    //string fileName = Path.GetFileNameWithoutExtension(s);
                    //new XmlParser(s).Generate(fileName + ".cs");
                    XmlParser.Generate(new XmlTextReader(s), Console.Out);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error: " + ex.Message + ex.StackTrace);
                }
            }

            return 0;
        }
    }
}
