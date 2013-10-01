using System;
using System.IO;
using System.Text;

namespace UavObjectGenerator
{
    public class CSharpGenerator
    {

        // __ Config consts ______________________________________________


        public const string Namespace = "UavTalk";



        public static void Write(TextWriter w, ObjectData obj)
        {
            WriteHeader(w, obj);
            WriteEnums(w, obj);
            WriteClassHeader(w, obj);
            WriteProperties(w, obj);
            WriteConstructor(w, obj);
            WriteSerialize(w, obj);
            WriteDeserialize(w, obj);
            WritePrivateFields(w, obj);
            WriteFooter(w, obj);
        }


        // __ Impl _______________________________________________________


        private static void WriteHeader(TextWriter w, ObjectData obj)
        {
            w.Write(string.Format(@"
using System;
using System.IO;
using UavTalk;

namespace {0}
{{

",
                Namespace));
        }

        private static void WriteClassHeader(TextWriter w, ObjectData obj)
        {
            w.Write(string.Format(@"
    public class {0}: UavDataObject
    {{

", 
                obj.Name));
        }

        private static void WriteProperties(TextWriter w, ObjectData obj)
        {
            foreach (FieldData f in obj.Fields)
            {
                w.Write(string.Format(@"
        public {2}{3} {0} {{
            get {{ return {1}; }}
            set {{ {1} = value; NotifyUpdated(); }}
        }}
",
                    f.Name, GetPrivateFieldName(f), GetCSharpType(obj, f), GetArrayModifier(f, false)));
            }
        }


        private static void WriteEnums(TextWriter w, ObjectData obj)
        {
            foreach (FieldData f in obj.Fields)
            {
                if (f.Type != "enum") continue;

                w.WriteLine(string.Format("    public enum {0} {{ {1} }};\n", 
                    GetEnumName(obj, f), GetEnumItems(f)));
            }
        }


        private static void WriteConstructor(TextWriter w, ObjectData obj)
        {

        }


        private static void WriteSerialize(TextWriter w, ObjectData obj)
        {
            w.Write(@"
        public override void Serialize(BinaryWriter stream)
        {
");
            foreach (FieldData f in obj.Fields)
            {
                w.WriteLine(string.Format("            stream.Write({0}{1});", 
                    GetSerializeTypeCast(obj, f), GetPrivateFieldName(f)));
            }

            w.WriteLine("        }\n");
        }

        private static void WriteDeserialize(TextWriter w, ObjectData obj)
        {
            w.Write(@"
        public override UavDataObject Deserialize(BinaryReader stream)
        {
");
            w.WriteLine(string.Format(@"            {0} result = new {0}();", obj.Name));

            foreach (FieldData f in obj.Fields)
            {
                w.WriteLine(string.Format("            {0} = {1}stream.{2}();", 
                        GetPrivateFieldName(f), GetEnumTypeCast(obj, f), GetReadOperation(f)));
            }

            w.WriteLine("            return result;");
            w.WriteLine("        }\n");
        }

        private static void WritePrivateFields(TextWriter w, ObjectData obj)
        {
            foreach (FieldData f in obj.Fields)
            {
                w.WriteLine(string.Format(@"        private {0}{1} {2}{3};", 
                    GetCSharpType(obj, f), GetArrayModifier(f, false), GetPrivateFieldName(f), GetDefaultValue(f)));
            }
        }

        private static void WriteFooter(TextWriter w, ObjectData obj)
        {
            w.Write(@"
    }
}
");
        }


        private static string GetDefaultValue(FieldData f)
        {   
            // = new UInt16[3]  // Roll, Pitch, Yaw
            return "";
        }


        private static int GetNumberOfElements(FieldData f)
        {
            if (f.ElementNames.Count > 0) return f.ElementNames.Count;

            if (f.Elements == null || f.Elements == "") return 0;

            int result; 
            if (Int32.TryParse(f.Elements, out result)) return result;

            return 0;
        }

        private static string GetArrayModifier(FieldData f, bool withNumberOfElements)
        {
            int numElements = GetNumberOfElements(f);

            if (numElements == 0) return "";

            return string.Format("[{0}]", (withNumberOfElements ? numElements.ToString() : ""));
        }

        private static string GetCSharpType(ObjectData obj, FieldData f)
        {
            switch (f.Type)
            {
                case "float": return "float";
                case "int8": 
                case "uint8": return "byte";
                case "int16": return "Int16";
                case "uint16": return "UInt16";
                case "enum": return GetEnumName(obj, f);
                case "int32": return "Int32";
                case "uint32": return "UInt32";
                default: 
                    Console.WriteLine("ERROR: Unknown uavType: " + f.Type);
                    return "!!!!";
            }
        }

        private static string GetSerializeTypeCast(ObjectData obj, FieldData f)
        {
            if (f.Type != "enum")
                return "";

            return "(byte)";
        }

        private static string GetEnumTypeCast(ObjectData obj, FieldData f)
        {
            if (f.Type != "enum")
                return "";

            return String.Format("({0})", GetEnumName(obj, f));
        }

        private static string GetReadOperation(FieldData f)
        {
            switch (f.Type)
            {
                case "float":  return "ReadSingle";
                case "int8":   return "ReadByte";
                case "uint8":  return "ReadByte";
                case "int16":  return "ReadInt16";
                case "int32":  return "ReadInt32";
                case "uint32": return "ReadUInt32";
                case "enum":   return "ReadByte";
                default:
                    Console.WriteLine("ERROR: Unknown uavType: " + f.Type);
                    return "!!!!";
            }
        }

        private static string GetEnumName(ObjectData obj, FieldData f)
        {
            return string.Format("{0}_{1}", obj.Name, f.Name);
        }

        private static string GetEnumItems(FieldData f)
        {
            if (f.Type != "enum") return "";

            StringBuilder result = new StringBuilder();
            bool isFirst = true;

            foreach (String s in f.Options)
            {
                if (isFirst)
                    isFirst = false;
                else
                    result.Append(", ");

                result.Append(s);
            }

            return result.ToString();
        }

        private static string GetPrivateFieldName(FieldData f)
        {
            return string.Format("m{0}", f.Name);
        }

    }
}

