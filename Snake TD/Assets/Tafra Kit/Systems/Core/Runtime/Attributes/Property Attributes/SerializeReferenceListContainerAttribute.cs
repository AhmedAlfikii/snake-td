using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class SerializeReferenceListContainerAttribute : PropertyAttribute
    {
        private readonly string listPropertyName;
        private bool uniqueElements;
        private string singleElementName;
        private string pluralElementName;
        private int maxElementsCount;

        public string ListPropretyName => listPropertyName;
        public bool ForceUniqueElements => uniqueElements;
        public string SingleElementName => singleElementName;
        public string PluralElementName => pluralElementName;
        public int MaxElementsCount => maxElementsCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listPropertyName">The name of the property that is a serialized reference list inside this object.</param>
        /// <param name="uniqueElements">If true, the list won't accept more than one element of the same type.</param>
        /// <param name="singleElementName"></param>
        /// <param name="pluralElementName"></param>
        /// <param name="maxElementsCount">The maximum number of elements that can be added to the list. 0 means unlimited.</param>
        public SerializeReferenceListContainerAttribute(string listPropertyName, bool uniqueElements = false, string singleElementName = "Element", string pluralElementName = "Elements", int maxElementsCount = 0)
        { 
            this.listPropertyName = listPropertyName;
            this.uniqueElements = uniqueElements;
            this.singleElementName = singleElementName;
            this.pluralElementName = pluralElementName;
            this.maxElementsCount = maxElementsCount;
        }
    }
}