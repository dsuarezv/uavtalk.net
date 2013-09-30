using System;
using System.Collections.Generic;

namespace UavObjectGenerator
{
    public class FieldData
    {   
        public string Name { get; set; }
        public string Type { get; set; }
        public string Units { get; set; }
        public string Elements { get; set; }
        public List<string> ElementNames { get; set; } 
        public List<string> Options { get; set; }
        public List<string> DefaultValues { get; set; }

        public FieldData()
        {
            ElementNames = new List<string>();
            Options = new List<string>();
            DefaultValues = new List<string>();
        }

        public void ParseElementNamesFromAttribute(string elementNamesAttribute)
        {
            ParseItemsIntoList(elementNamesAttribute, ElementNames);
        }

        public void ParseOptionsFromAttribute(string optionsAttribute)
        {
            ParseItemsIntoList(optionsAttribute, Options);
        }

        public void ParseDefaultValuesFromAttribute(string defaultValuesAttribute)
        {
            ParseItemsIntoList(defaultValuesAttribute, DefaultValues);

            if (DefaultValues.Count == 1 && ElementNames.Count > 1)
            {
                //Only one default value given: apply it to all the elements
                for (int i = 1; i < ElementNames.Count; ++i)
                {
                    DefaultValues.Add(DefaultValues[0]);
                }
            }
        }


        private void ParseItemsIntoList(string items, List<string> target)
        {
            if (items == null || items == "")
                return;

            string[] ss = items.Split(',');

            foreach (string s in ss)
            {
                target.Add(s.Trim());
            }
        }
    }
}

