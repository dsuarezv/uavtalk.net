using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
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

            try
            {

                Configuration c = new Configuration(args);

                c.CheckValid();
               

                foreach (string s in c.Files)
                {
                    try
                    {
                        string fileName = Path.GetFileNameWithoutExtension(s);
                        string outputFileName = Path.Combine(c.OutputDir, fileName + ".cs");
                        new XmlParser(s).Generate(outputFileName);
                        //XmlParser.Generate(new XmlTextReader(s), Console.Out);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("Error in [{0}]: {1} ", s, ex.Message);
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }




    }
}
