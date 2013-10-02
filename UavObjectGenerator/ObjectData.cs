using System;
using System.Collections.Generic;

namespace UavObjectGenerator
{
    public class ObjectData
    {
        public string Name;
        public string Description;
        public List<FieldData> Fields = new List<FieldData>();
        public Dictionary<string, FieldData> FieldsIndex = new Dictionary<string, FieldData>();
    }
}

