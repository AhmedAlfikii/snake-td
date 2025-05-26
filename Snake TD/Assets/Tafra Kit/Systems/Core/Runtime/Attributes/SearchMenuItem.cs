using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class SearchMenuItem : Attribute
    {
        private string menuName;

        public string MenuName => menuName;

        public SearchMenuItem(string menuName)
        {
            this.menuName = menuName;
        }
    }
}
