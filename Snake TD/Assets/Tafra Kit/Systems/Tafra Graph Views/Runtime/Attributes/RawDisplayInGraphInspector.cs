using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.GraphViews
{
    /// <summary>
    /// Use this on fields you don't want the graph inspector to manually fetch its child fields and display it, but instead display it as it is (useful if it has a custom drawer).
    /// </summary>
    public class RawDisplayInGraphInspector : Attribute
    {

    }
}