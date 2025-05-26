using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.GraphViews
{
    public class GraphNodeName : Attribute
    {
        public string Name;
        public string ShortName;

        public GraphNodeName(string name)
        {
            Name = name;
            ShortName = name;
        }
        public GraphNodeName(string name, string shortName)
        {
            Name = name;
            ShortName = shortName;
        }
    }
}