using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.MetaGame;
using TafraKit.SceneManagement;
using System;

namespace TafraKit.Demos
{
    public class DailyChallengeLevelDemo : MonoBehaviour
    {
        private void Start()
        {
            DailyChallenges.RegisterOnChallengeCloseLoadParam("home_menu_index", 2);
        }

        public void Win()
        {
            DailyChallenges.RegisterOnChallengeCloseLoadParam("challenge_completed", DailyChallenges.OpenedChallenge);
            DailyChallenges.CompleteOpenedChallenge();
        }
        public void Fail()
        {
            DailyChallenges.FailOpenedChallenge();
        }
    }
}