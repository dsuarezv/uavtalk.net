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
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("Error in [{0}]: {1} ", s, ex.Message);
                    }
                }

                SummaryGenerator.Write(c.OutputDir);

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
