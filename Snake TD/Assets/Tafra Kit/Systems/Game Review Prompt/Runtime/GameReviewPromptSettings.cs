using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.ReviewPrompt
{
    public class GameReviewPromptSettings : SettingsModule
    {
        public bool Enabled;
        [Tooltip("The level where the attempts to display a review prompt on its complete screen will start at (if it fails, then it will try again with the next level until it succeeds).")]
        public int Level;
        [Tooltip("Android only, disable this if the duration between the level start and end is too long since the review info could expire. You'll then have to manually order to fetch it somewhere near the level's end.")]
        public bool GetReviewInfoOnLevelStart = true;
        public override int Priority => 7;

        public override string Name => "Mobile/Game Review Prompt";

        public override string Description => "Displays a review prompt for users to rate your game.";
    }
}