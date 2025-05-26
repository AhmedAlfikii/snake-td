using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Narrative
{
    public class ConversationSettings : SettingsModule
    {
        public bool Enabled;

        [Header("References")]
        [Tooltip("The canvas that will be instantiated on game start, and all the speech containers will spawn inside it.")]
        public Canvas speechContainersCanvas;
        [Tooltip("The container that will be used whenever a speech start above a character's head.")]
        public SpeechContainer overHeadSpeechContainer;
        [Tooltip("The container that will be used whenever a speech start on screen (not bound to a spearker's transform).")]
        public SpeechContainer onScreenSpeechContainer;

        [Header("Default Properties")]
        [SerializeField] public float DurationBetweenLetters = 0.05f;

        public override int Priority => 14;

        public override string Name => "Narration/Conversation";

        public override string Description => "Control conversations between characters.";
    }
}