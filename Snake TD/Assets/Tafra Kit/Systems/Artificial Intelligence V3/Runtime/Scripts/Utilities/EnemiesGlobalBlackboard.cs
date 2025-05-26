using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.AI3;
using UnityEngine;
using UnityEngine.SceneManagement;
using TafraKit.GraphViews;
using TafraKit.ContentManagement;

namespace TafraKit
{
    public class EnemiesGlobalBlackboard : MonoBehaviour
    {
        [SerializeField] private TafraAsset<ExternalBlackboard> blackboardReference;

        private GenericExposableProperty<TafraKit.TafraActor> playerProperty;

        private void Awake()
        {
            var bb = blackboardReference.Load();

            bb.Initialize();
            playerProperty = bb.TryGetActorProperty(Animator.StringToHash("Target Actor"), -1);
            SetPlayerProperty();

        }
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        private void OnDestroy()
        {
            blackboardReference.Release();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            SetPlayerProperty();
        }

        private void SetPlayerProperty()
        {
            if(playerProperty == null)
                return;

            playerProperty.value = FindAnyObjectByType<PlayableActor>();
        }
    }
}