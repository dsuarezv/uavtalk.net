using System;
using System.Collections.Generic;

namespace UavObjectGenerator
{
    public class ObjectData
    {
        public string Name;
        public string Description;
        public int IsSettingsInt;
        public int IsSingleInstInt;
        public List<FieldData> Fields = new List<FieldData>();
        public Dictionary<string, FieldData> FieldsIndexedByName = new Dictionary<string, FieldData>();
    }
}

