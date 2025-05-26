using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.GraphViews
{
    [System.Serializable]
    public class GenericExposableProperty<T> : ExposableProperty
    {
        public T value;

        public GenericExposableProperty()
        { 

        }
        public GenericExposableProperty(string name, string tooltip, T value, int id) : base(name, tooltip, id)
        {
            this.value = value;
            this.id = id;
        }        
        public GenericExposableProperty(string name, T value, int id) : base(name, id)
        {
            this.value = value;
            this.id = id;
        }        
    }
}