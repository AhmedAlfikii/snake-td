using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.RPG
{
    [CreateAssetMenu(menuName = "Tafra Kit/RPG/Storable", fileName = "Storable")]
    public class StorableScriptableObject : IdentifiableScriptableObject, IInstanceableScriptableObject
    {
        [SerializeField] protected TafraString category;
        [SerializeField] protected bool isStackable;

        [NonSerialized] protected int instanceNumber;
        [NonSerialized] protected int quantity = 1;
        [NonSerialized] protected bool isInstance;
        [NonSerialized] protected UnityEvent onQuantityChange = new UnityEvent();

        public override string ID { get { return name; } }
        public string Category => category.Value;
        public bool IsStackable => isStackable;
        public int Quantity
        { 
            get 
            { 
                return quantity; 
            }
            set 
            {
                quantity = value;
                onQuantityChange?.Invoke();
            }
        }

        public UnityEvent OnQuantityChange => onQuantityChange;
        public string OriginalID => ID;
        public string InstanceID => InstancableSO.GetSOInstanceID();
        public int InstanceNumber
        {
            get { return instanceNumber; }
            set { instanceNumber = value; }
        }
        public bool IsInstance { get => isInstance; set { isInstance = value; } }
        public ScriptableObject OriginalScriptableObject { get; set; }
        public IInstanceableScriptableObject InstancableSO => this;

        /// <summary>
        /// Use on instances of this storable to save their data. Note: only use on instances, do not use on original objects.
        /// </summary>
        public virtual void Save()
        {
            PlayerPrefs.SetInt($"STORABLE_{InstancableSO.GetSOInstanceID()}_QUANTITY", quantity);
        }
        /// <summary>
        /// Use on instances of this storable to load their data. Note: only use on instances, do not use on original objects.
        /// </summary>
        public virtual void Load()
        { 
            quantity = PlayerPrefs.GetInt($"STORABLE_{InstancableSO.GetSOInstanceID()}_QUANTITY", 1);
        }
        /// <summary>
        /// Use this before abandoning this instance to free up its occupied space in the device storage. Note: only use on instances, do not use on original objects.
        /// </summary>
        public virtual void ClearSavedKeys()
        {
            PlayerPrefs.DeleteKey($"STORABLE_{InstancableSO.GetSOInstanceID()}_QUANTITY");
        }
    }
}