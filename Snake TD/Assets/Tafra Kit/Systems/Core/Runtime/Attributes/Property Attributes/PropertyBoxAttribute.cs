using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class PropertyBoxAttribute : PropertyAttribute
    {
        private readonly string title;
        private readonly string subtitle;
        private readonly string tooltip;
        private readonly bool dontDrawContent;
        private readonly bool drawPropertyInContent;


        public string Title => title;
        public string Subtitle => subtitle;
        public string Tooltip => tooltip;
        public bool DontDrawContent => dontDrawContent;
        public bool DrawPropertyInContent => drawPropertyInContent;

        /// <summary>
        /// Draw a property box.
        public PropertyBoxAttribute()
        { 
        }
        /// <summary>
        /// Draw a property box.
        /// </summary>
        /// <param name="subtitle">The subtitle to display below the main title. Send an empty string if you don't want a subtitle.</param>
        public PropertyBoxAttribute(string subtitle)
        { 
            this.subtitle = subtitle;
        }
        /// <summary>
        /// Draw a property box.
        /// </summary>
        /// <param name="title">The title to display on the box.</param>
        /// <param name="subtitle">The subtitle to display below the main title. Send an empty string if you don't want a subtitle.</param>
        /// <param name="tooltip">The tooltip that will be displayed when the user hovers over the header.</param>
        /// <param name="dontDrawContent">Prevent the box from automatically drawing the content of the property (this means you'll be drawing it by yourself).</param>
        /// <param name="drawPropertyInContent">If dontDrawContent is disabled. Should the content contain the property itself? 
        /// If false, the box will go through each of the property's children and draw them one by one instead of drawing the property as a whole.</param>
        public PropertyBoxAttribute(string title, string subtitle, string tooltip, bool dontDrawContent = false, bool drawPropertyInContent = false)
        { 
            this.title = title;
            this.subtitle = subtitle;
            this.tooltip = tooltip;
            this.dontDrawContent = dontDrawContent;
            this.drawPropertyInContent = drawPropertyInContent;
        }
    }
}