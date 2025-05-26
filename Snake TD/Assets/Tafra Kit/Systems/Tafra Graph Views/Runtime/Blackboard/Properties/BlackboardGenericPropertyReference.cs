using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.GraphViews
{
    public abstract class BlackboardGenericPropertyReference<T> : BlackboardPropertyReference
    {
        [NonSerialized] protected GenericExposableProperty<T> property;
        protected abstract GenericExposableProperty<T> GetProperty();

        public BlackboardGenericPropertyReference() { }
        public BlackboardGenericPropertyReference(BlackboardGenericPropertyReference<T> other) : base(other)
        {
            property = other.property;
        }

        public GenericExposableProperty<T> Property => property;

        public void CacheProperty()
        {
            property = GetProperty();
        }
    }
}