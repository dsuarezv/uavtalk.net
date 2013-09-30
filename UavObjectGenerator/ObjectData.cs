using System;
using System.Collections.Generic;

namespace UavObjectGenerator
{
    public class ObjectData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<FieldData> Fields { get; set; }

        public ObjectData()
        {
            Fields = new List<FieldData>();
        }


    }
}

