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
            WriteToString(w, obj);
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
                if (!f.IsEnum) continue;

                WL(w, "    public enum {0} {{ {1} }};", GetEnumName(obj, f, false), GetEnumItems(f));
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

        private static void WriteToString(TextWriter w, ObjectData obj)
        {
            WL(w, "        public override string ToString()");
            WL(w, "        {");
            WL(w, "            System.Text.StringBuilder sb = new System.Text.StringBuilder();");
            WL(w);
            WL(w, "            sb.Append(\"{0} \\n\");", obj.Name);

            foreach (FieldData f in obj.Fields)
            {
                if (f.NumElements == 1)
                {
                    WL(w, "            sb.AppendFormat(\"    {0}: {{0}} {1}\\n\", {0});", f.Name, f.Units);
                }
                else
                {
                    WL(w, "            sb.Append(\"    {0}\\n\");", f.Name);
                    for (int i = 0; i < f.NumElements; ++i)
                    {
                        string elemName = (f.ElementNames.Count == f.NumElements) ? f.ElementNames[i] : "";
                        WL(w, "            sb.AppendFormat(\"        {1}: {{0}} {3}\\n\", {0}[{2}]);", f.Name, elemName, i, f.Units);
                    }
                }
            }

            WL(w);
            WL(w, "            return sb.ToString();");
            WL(w, "        }");
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
            // Cases:
            // - Single value
            //   - float n = 0.0f
            //   - MyEnum n = MyEnum.Value
            // - Array value
            //   - float[] n = new float[3];    // Roll, Pitch, Yaw
            //   - float[] n = new float[3] { 0.1f, 2f, 0.3f };   // Roll, Pitch, Yaw
            //   - MyEnum[] n = new MyEnum[3] { MyEnum.Value, MyEnum.Value };   // RollMode, PitchMode

            if (f.NumElements <= 1)
            {
                // Single value
                if (f.DefaultValues.Count == 1)
                {
                    return " = " + GetFormattedDefaultValue(obj, f, 0);
                }
            }
            else
            {
                // Array value
                return string.Format(" = new {0}[{1}] {2}",
                    GetCSharpType(obj, f), f.NumElements, GetDefaultValuesList(obj, f));
            }

            return "";
        }

        private static string GetFormattedDefaultValue(ObjectData obj, FieldData f, int index)
        {
            if (f.IsEnum)
            {
                return string.Format("{0}.{1}", GetEnumName(obj, f, false), FieldData.GetEscapedItemName(f.DefaultValues[index]));
            }
            else
            {
                return string.Format("{0}{1}", f.DefaultValues[index], GetFieldTypeSuffix(f));
            }
        }

        private static string GetBracketedString(string s)
        {
            return string.Format("{{ {0} }}", s);
        }

        private static string GetDefaultValuesList(ObjectData obj, FieldData f)
        {

            // Case 0: No default values: just return empty.

            if (f.DefaultValues.Count == 0) return "";

            // Case 1: there is a default value for every item

            if (f.DefaultValues.Count == f.NumElements)
            {
                if (f.IsEnum)
                    return GetBracketedString(GetEnumCommaSeparatedValues(GetEnumName(obj, f, false), f.DefaultValues));
                else
                    return GetBracketedString(GetCommaSeparatedValues(f.DefaultValues, GetFieldTypeSuffix(f)));
            }

            // Case 2: there is only one default value that is applied to all items. 
            //   Expand the given value to a list and apply to all items.

            /*
            if (f.DefaultValues.Count == 1)
            {
                List<string> expandedDefaults = new List<string>();
                string valueToExpand = GetFormattedDefaultValue(obj, f, 0);

                if (f.Type == FieldDataType.UINT8 && f.NumElements == valueToExpand.Length)
                {
                    // Special case: array of uint8 as chars
                    for (int i = 0; i < f.NumElements; ++i)
                    {
                        expandedDefaults.Add(string.Format("0x{0:x2}", (int)valueToExpand[i]));
                    }
                }
                else
                {
                    // Create a list expanding the same value to the given number of items
                    for (int i = 0; i < f.NumElements; ++i)
                    {
                        expandedDefaults.Add(string.Format("{0}{1}", GetEnumName(obj, f, true), valueToExpand));
                    }
                }

                return GetBracketedString(GetCommaSeparatedValues(expandedDefaults, ""));
            }
            */

            return "";
        }

        private static string GetFieldTypeSuffix(FieldData f)
        {
            switch (f.Type)
            {
                case FieldDataType.FLOAT32:
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
                result.Add(string.Format("{0}.{1}", enumName, FieldData.GetEscapedItemName(list[i])));
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
                case "enum": return GetEnumName(obj, f, false);
                case "int32": return "Int32";
                case "uint32": return "UInt32";
                default: 
                    Console.WriteLine("ERROR: Unknown uavType: " + f.TypeString);
                    return "!!!!";
            }
        }

        private static string GetSerializeTypeCast(ObjectData obj, FieldData f)
        {
            if (!f.IsEnum)
                return "";

            return "(byte)";
        }

        private static string GetEnumTypeCast(ObjectData obj, FieldData f)
        {
            if (!f.IsEnum)
                return "";

            return String.Format("({0})", GetEnumName(obj, f, false));
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

        private static string GetEnumName(ObjectData obj, FieldData f, bool includeDot)
        {
            if (!f.IsEnum) return "";

            return string.Format("{0}_{1}{2}", obj.Name, f.Name, (includeDot ? "." : "") );
        }

        private static string GetEnumItems(FieldData f)
        {
            if (!f.IsEnum) return "";

            List<string> escapedEnum = new List<string>();
            foreach (string s in f.Options)
                escapedEnum.Add(FieldData.GetEscapedItemName(s));

            return GetCommaSeparatedValues(escapedEnum, "");
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

