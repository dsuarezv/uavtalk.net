using System;
using System.IO;
using System.Collections.Generic;

namespace UavObjectGenerator
{
    /* Pendiente: 
     * - Hay que añadir singleinstance a cada objeto, para el parseo del campo instanceid
     * - Hay que cambiar el sistema de registro de object ids. Con esta clase se registran
     *     los tipos en un nuevo fichero (ObjectSummary.cs)
     * - Hay que acabar el programa de ejemplo.
     *
     */

    public class SummaryGenerator
    {
        public static void RegisterObjectId(UInt32 id, string className)
        {
            mObjectIds[id] = className;
        }

        public static void Write(string outputDir)
        {
            string fileName = Path.Combine(outputDir, "ObjectSummary.cs");

            using (TextWriter w = new StreamWriter(fileName))
            {
                WriteHeader(w);
                WriteGetTypeForId(w);
                WriteRegisterObjects(w);
                WriteFooter(w);
            }
        }


        // __ Impl _______________________________________________________


        private static void WL(TextWriter w)
        {
            CSharpGenerator.WL(w);
        }

        private static void WL(TextWriter w, string s, params object[] args)
        {
            CSharpGenerator.WL(w, s, args);
        }


        // __ Code generators _____________________________________________


        private static void WriteHeader(TextWriter w)
        {
            WL(w, "using System;");
            WL(w, "using System.IO;");
            WL(w, "using UavTalk;");
            WL(w);
            WL(w, "namespace {0}", CSharpGenerator.Namespace);
            WL(w, "{");
            WL(w);
            WL(w, "    public partial class ObjectSummary");
            WL(w, "    {");

        }

        private static void WriteGetTypeForId(TextWriter w)
        {
            WL(w, "        public static UavDataObject CreateObject(UInt32 id)");
            WL(w, "        {");
            WL(w, "              switch (id)");
            WL(w, "              {");
        
            foreach (KeyValuePair<UInt32, String> entry in mObjectIds)
            {
                WL(w, "                case 0x{0:x8}: return new {1}();", entry.Key, entry.Value);
            }

            WL(w, "            }");
            WL(w, "            return null;");
            WL(w, "        }");
            WL(w);
        }

        private static void WriteRegisterObjects(TextWriter w)
        {
            WL(w, "        public static void RegisterObjects()");
            WL(w, "        {");

            foreach (KeyValuePair<UInt32, String> entry in mObjectIds)
            {
                WL(w, "            RegisterObjectType(0x{0:x8}, typeof({1}));", entry.Key, entry.Value);
            }

            WL(w, "        }");
            WL(w);
        }

        private static void WriteFooter(TextWriter w)
        {
            WL(w, "    }");
            WL(w, "}");
        }

        private static Dictionary<UInt32, String> mObjectIds = new Dictionary<uint, String>();
    }
}

