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
					Generate(reader, writer);
				}
			}
		}

		public static void Generate(XmlTextReader reader, StreamWriter writer)
		{
			ObjectData data = GetObjectFromXml(reader);

			CSharpGenerator.Write(writer, data);
		}


		// __ Impl _______________________________________________________


		private static ObjectData GetObjectFromXml(XmlTextReader reader)
		{
			ObjectData result = new ObjectData();

			while (reader.Read())
			{
				if (reader.IsStartElement())
				{
					switch (reader.Name)
					{
						case "object":
							result.Name = reader.GetAttribute("name");
							break;
						case "description": 
							result.Description = reader.ReadString();							
							break;
						case "field":
							FieldData field = new FieldData();
							field.Name = reader.GetAttribute("name");
							field.Type = reader.GetAttribute("type");
							field.Elements = reader.GetAttribute("elements");
							field.Units = reader.GetAttribute("units");
							result.Fields.Add(field);
							break;
					}
				}
			}

			return result;
		}



		private string mSourceFileName;
    }
}

