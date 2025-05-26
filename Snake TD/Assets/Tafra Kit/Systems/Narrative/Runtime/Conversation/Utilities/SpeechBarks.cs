using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Narrative
{
    [CreateAssetMenu(fileName = "Speech Barks", menuName = "Tafra Kit/Conversation/Speech Barks")]
    public class SpeechBarks : SOItemsPool<SpeechData>
    {
        [Tooltip("Barks that will be returned in a sequence, one after the other.")]
        [SerializeField] private List<SpeechData> sequentialBarks;
        [Tooltip("If the sequential barks are all requested, whenever you request a new bark, one element of this list will be returned.")]
        [SerializeField] private List<SpeechData> randomBarks;

        protected override List<SpeechData> SequentialItems => sequentialBarks;

        protected override List<SpeechData> RandomItems => randomBarks;
    }
}