using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Weaponry
{
    [Serializable]
    public class ProjectileExtension
    {
        protected Projectile projectile;

        private Action onUnbinded;

        public Action OnUnbinded { get => onUnbinded; set => onUnbinded = value; }

        public ProjectileExtension() { }
        public void Bind(Projectile projectile)
        { 
            this.projectile = projectile;

            OnBind();
        }
        public void Unbind()
        {
            onUnbinded?.Invoke();

            OnUnbind();
        }

        /// <summary>
        /// Copy the values of this extension to another.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void CopyTo(ProjectileExtension other)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnBind()
        { 

        }
        protected virtual void OnUnbind()
        { 

        }
    }
}