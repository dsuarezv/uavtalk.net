using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

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


        internal static void WL(TextWriter w)
        {
            w.WriteLine();
        }

        internal static void WL(TextWriter w, string s, params object[] args)
        {
            if (args.Length == 0)
                w.WriteLine(s);
            else
                w.WriteLine(string.Format(s, args));
        }


        // __ Code generators _____________________________________________


        private static void WriteHeader(TextWriter w, ObjectData obj)
        {
            WL(w, "using System;");
            WL(w, "using System.IO;");
            WL(w, "using UavTalk;");
            WL(w);
            WL(w, "namespace {0}", Namespace);
            WL(w, "{");
            WL(w);
        }


        private static void WriteClassHeader(TextWriter w, ObjectData obj)
        {
            WL(w, "    public class {0}: UavDataObject", obj.Name);
            WL(w, "    {");
        }


        private static void WriteProperties(TextWriter w, ObjectData obj)
        {
            foreach (FieldData f in obj.Fields)
            {
                WL(w, "        public {0}{1} {2} {{", GetCSharpType(obj, f), GetArrayModifier(f, false), f.Name);
                WL(w, "            get {{ return {0}; }}", GetPrivateFieldName(f));
                WL(w, "            set {{ {0} = value; NotifyUpdated(); }}", GetPrivateFieldName(f));
                WL(w, "        }");
                WL(w);
            }
        }


        private static void WriteEnums(TextWriter w, ObjectData obj)
        {
            foreach (FieldData f in obj.Fields)
            {
                if (f.TypeString != "enum") continue;

                WL(w, "    public enum {0} {{ {1} }};", GetEnumName(obj, f), GetEnumItems(f));
                WL(w);
            }
        }


        private static void WriteConstructor(TextWriter w, ObjectData obj)
        {
            WL(w, "        public {0}()", obj.Name);
            WL(w, "        {");
            WL(w, "            IsSingleInstance = {0};", (obj.IsSingleInstInt == 1) ? "true" : "false");
            WL(w, "            ObjectId = 0x{0:x8};", Hasher.CalculateId(obj));
            WL(w, "        }");
            WL(w);
        }


        private static void WriteSerialize(TextWriter w, ObjectData obj)
        {
            WL(w, "        internal override void SerializeBody(BinaryWriter s)");
            WL(w, "        {");

            foreach (FieldData f in obj.Fields)
            {
                int numElements = f.NumElements;

                if (numElements <= 1)
                {
                    WL(w, "            s.Write({0}{1});", GetSerializeTypeCast(obj, f), GetPrivateFieldName(f));
                }
                else
                {
                    for (int i = 0; i < numElements; ++i)
                    {
                        WL(w, "            s.Write({0}{1}[{2}]);  // {3}", 
                           GetSerializeTypeCast(obj, f), GetPrivateFieldName(f), i, GetElementNameAt(f, i));
                    }
                }

            }

            WL(w, "        }\n");
            WL(w);
        }

        private static void WriteDeserialize(TextWriter w, ObjectData obj)
        {
            WL(w, "        internal override void DeserializeBody(BinaryReader stream)", obj.Name);
            WL(w, "        {");

            foreach (FieldData f in obj.Fields)
            {
                int numElements = f.NumElements;

                if (numElements <= 1)
                {
                    WL(w, "            this.{0} = {1}stream.{2}();", 
                       GetPrivateFieldName(f), GetEnumTypeCast(obj, f), GetReadOperation(f));
                }
                else
                {
                    for (int i = 0; i < numElements; ++i)
                    {
                        WL(w, "            this.{0}[{1}] = {2}stream.{3}();  // {4}", 
                           GetPrivateFieldName(f), i, GetEnumTypeCast(obj, f), GetReadOperation(f), GetElementNameAt(f, i));
                    }
                }
            }

            WL(w, "        }\n");
            WL(w);
        }

        private static void WritePrivateFields(TextWriter w, ObjectData obj)
        {
            foreach (FieldData f in obj.Fields)
            {
                WL(w, "        private {0}{1} {2}{3};", 
                   GetCSharpType(obj, f), GetArrayModifier(f, false), 
                   GetPrivateFieldName(f), GetDefaultValue(obj, f));
            }
        }

        private static void WriteFooter(TextWriter w, ObjectData obj)
        {
            WL(w, "    }");
            WL(w, "}");
        }


        // __ Helpers _____________________________________________________________


        private static string GetElementNameAt(FieldData f, int index)
        {
            if (index < f.ElementNames.Count) return f.ElementNames[index];

            return "NO_ELEMENT_NAME";
        }


        private static string GetDefaultValue(ObjectData obj, FieldData f)
        {   
            // "= new UInt16[3]  // Roll, Pitch, Yaw"

            int numElements = f.NumElements;

            if (numElements <= 1)
            {
                if (f.DefaultValues.Count == 1)
                {
                    if (f.TypeString == "enum")
                    {
                        return string.Format(" = {0}.{1}", GetEnumName(obj, f), f.DefaultValues[0]);
                    }
                    else
                    {
                        return string.Format(" = {0}", f.DefaultValues[0] + GetFieldTypeSuffix(f));
                    }
                }
            }
            else
            {
                return string.Format(" = new {0}[{1}] {2}",
                    GetCSharpType(obj, f), numElements, GetDefaultValuesList(obj, f));
            }

            return "";
        }

        private static string GetBracketedString(string s)
        {
            return string.Format("{{ {0} }}", s);
        }

        private static string GetDefaultValuesList(ObjectData obj, FieldData f)
        {
            if (f.DefaultValues.Count == 0) return "";

            if (f.DefaultValues.Count == f.ElementNames.Count)
            {
                if (f.TypeString == "enum")
                    return GetBracketedString(GetEnumCommaSeparatedValues(GetEnumName(obj, f), f.DefaultValues));
                else
                    return GetBracketedString(GetCommaSeparatedValues(f.DefaultValues, GetFieldTypeSuffix(f)));
            }

            if (f.DefaultValues.Count == 1)
            {
                List<string> expandedDefaults = new List<string>();

                string enumName = f.TypeString == "enum" ? GetEnumName(obj, f) + '.' : "";
                string valueToExpand = f.DefaultValues[0];
                int numElements = f.NumElements;

                if (f.TypeString == "uint8" && numElements == valueToExpand.Length)
                {
                    // Special case: array of uint8 as chars
                    for (int i = 0; i < numElements; ++i)
                    {
                        expandedDefaults.Add(string.Format("0x{0:x2}", (int)valueToExpand[i]));
                    }
                }
                else
                {
                    // Create a list expanding the same value to the given number of items
                    for (int i = 0; i < numElements; ++i)
                    {
                        expandedDefaults.Add(string.Format("{0}{1}", enumName, valueToExpand));
                    }
                }

                return GetBracketedString(GetCommaSeparatedValues(expandedDefaults, GetFieldTypeSuffix(f)));
            }

            return "";
        }

        private static string GetFieldTypeSuffix(FieldData f)
        {
            switch (f.TypeString)
            {
                case "float":
                    return "f";
                default:
                    return "";
            }
        }

        private static string GetEnumCommaSeparatedValues(string enumName, List<string> list)
        {
            List<string> result = new List<string>();

            for (int i = 0; i < list.Count; ++i)
            {
                result.Add(string.Format("{0}.{1}", enumName, list[i]));
            }

            return GetCommaSeparatedValues(result, "");
        }

        private static string GetArrayModifier(FieldData f, bool withNumberOfElements)
        {
            int numElements = f.NumElements;

            if (numElements <= 1) return "";

            return string.Format("[{0}]", (withNumberOfElements ? numElements.ToString() : ""));
        }

        private static string GetCSharpType(ObjectData obj, FieldData f)
        {
            switch (f.TypeString)
            {
                case "float": return "float";
                case "int8": return "SByte";
                case "uint8": return "byte";
                case "int16": return "Int16";
                case "uint16": return "UInt16";
                case "enum": return GetEnumName(obj, f);
                case "int32": return "Int32";
                case "uint32": return "UInt32";
                default: 
                    Console.WriteLine("ERROR: Unknown uavType: " + f.TypeString);
                    return "!!!!";
            }
        }

        private static string GetSerializeTypeCast(ObjectData obj, FieldData f)
        {
            if (f.TypeString != "enum")
                return "";

            return "(byte)";
        }

        private static string GetEnumTypeCast(ObjectData obj, FieldData f)
        {
            if (f.TypeString != "enum")
                return "";

            return String.Format("({0})", GetEnumName(obj, f));
        }

        private static string GetReadOperation(FieldData f)
        {
            switch (f.TypeString)
            {
                case "float":  return "ReadSingle";
                case "int8":   return "ReadSByte";
                case "uint8":  return "ReadByte";
                case "int16":  return "ReadInt16";
                case "uint16": return "ReadUInt16";
                case "int32":  return "ReadInt32";
                case "uint32": return "ReadUInt32";
                case "enum":   return "ReadByte";
                default:
                    Console.WriteLine("ERROR: Unknown uavType: " + f.TypeString);
                    return "UNKNOWN_UAV_TYPE";
            }
        }

        private static string GetEnumName(ObjectData obj, FieldData f)
        {
            return string.Format("{0}_{1}", obj.Name, f.Name);
        }

        private static string GetEnumItems(FieldData f)
        {
            if (f.TypeString != "enum") return "";

            return GetCommaSeparatedValues(f.Options, "");
        }

        private static string GetCommaSeparatedValues(List<string> list, string suffix)
        {
            StringBuilder result = new StringBuilder();
            bool isFirst = true;

            foreach (String s in list)
            {
                if (isFirst)
                    isFirst = false;
                else
                    result.Append(", ");

                result.Append(s + suffix);
            }

            return result.ToString();
        }

        private static string GetPrivateFieldName(FieldData f)
        {
            return string.Format("m{0}", f.Name);
        }

    }
}

