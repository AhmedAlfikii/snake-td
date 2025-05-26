using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Narrative
{
    public class SpeakerTransform : MonoBehaviour
    {
        [SerializeField] private CharacterIdentity identity;
        [SerializeField] private Transform head;

        public CharacterIdentity Identity => identity;
        public Transform Head => head;
    }
}