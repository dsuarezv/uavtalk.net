using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

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

        public static void Generate(XmlTextReader reader, TextWriter writer)
        {
            ObjectData data = GetObjectFromXml(reader);

            CSharpGenerator.Write(writer, data);
        }


        // __ Impl _______________________________________________________


        private static ObjectData GetObjectFromXml(XmlTextReader reader)
        {
            ObjectData currentObject = null;
            FieldData currentField = null;

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "object":
                            currentObject = new ObjectData();    
                            currentObject.Name = reader.GetAttribute("name");
                            break;
                        case "description": 
                            currentObject.Description = reader.ReadString();                            
                            break;
                        case "field":
                            currentField = new FieldData();
                            currentField.Name = reader.GetAttribute("name");
                            currentObject.FieldsIndex.Add(currentField.Name, currentField);

                            if (IsClone(reader))
                            {
                                currentField.CloneFrom(currentObject.FieldsIndex[reader.GetAttribute("cloneof")]);
                            }
                            else
                            {
                                currentField.Type = reader.GetAttribute("type");
                                currentField.Elements = reader.GetAttribute("elements");
                                currentField.Units = reader.GetAttribute("units");
                                currentField.ParseElementNamesFromAttribute(reader.GetAttribute("elementnames"));
                                currentField.ParseOptionsFromAttribute(reader.GetAttribute("options"));
                                currentField.ParseDefaultValuesFromAttribute(reader.GetAttribute("defaultvalue"));
                                currentObject.Fields.Add(currentField);
                            }
                            break;
                        case "option": 
                            currentField.Options.Add(FieldData.GetFilteredItemName(reader.ReadString()));
                            break;
                        case "elementname": 
                            currentField.ElementNames.Add(reader.ReadString());
                            break;
                    }
                }
            }

            return currentObject;
        }

        private static bool IsClone(XmlReader reader)
        {
            string cloneOf = reader.GetAttribute("cloneof");

            return (cloneOf != null && cloneOf != "");
        }
        
        private string mSourceFileName;
    }
}

