using System;
using UavObjectGenerator;

namespace UavGen
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //XmlParser p = new XmlParser("/Users/dave/develop/Taulabs-dyquo/xmls/accels.xml");
            XmlParser p = new XmlParser("/Users/dave/develop/Taulabs-dyquo/xmls/brushlessgimbalsettings.xml");
           
            p.Generate("/Users/dave/develop/Taulabs-dyquo/output/accels.cs");
        }
    }
}
