using System;
using System.IO;
using System.Xml;

namespace UavObjectGenerator
{
    public class XmlParser
    {
        public XmlParser(string fileName)
        {
			mSourceFileName = fileName;
        }

		public void Generate(string targetFileName)
		{
			using (XmlTextReader reader = new XmlTextReader(mSourceFileName))
			{
				using (StreamWriter writer = new StreamWriter(targetFileName))
				{
					ProcessInput(reader);
				}
			}
		}



		private void ProcessInput(XmlReader reader)
		{

		}


		private string mSourceFileName;
    }
}

