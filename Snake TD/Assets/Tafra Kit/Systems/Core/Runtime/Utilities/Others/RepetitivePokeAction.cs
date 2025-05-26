using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public class RepetitivePokeAction : MonoBehaviour
    {
        [Tooltip("The id that will be used to save poking progress. If empty, the game object's name will be used instead.")]
        [SerializeField] private string id;

        [SerializeField] private int firstActionAfterPokes = 1;
        [SerializeField] private int repeatedActionAfterPokes = 3;

        public UnityEvent OnFirstAction;
        public UnityEvent OnAction;

        private string ID
        {
            get
            {
                if (!string.IsNullOrEmpty(id))
                    return id;
                else
                    return name;
            }
        }
        private int pokes;

        void Awake()
        {
            pokes = PlayerPrefs.GetInt($"POKES_{ID}");
        }

        public void Poke()
        {
            pokes++;

            PlayerPrefs.SetInt($"POKES_{ID}", pokes);

            if (pokes == firstActionAfterPokes)
            {
                OnFirstAction?.Invoke();
                OnAction?.Invoke();
            }
            else if (pokes > firstActionAfterPokes && (pokes - firstActionAfterPokes) % repeatedActionAfterPokes == 0)
                OnAction?.Invoke();
        }
    }
}