using UnityEngine;

namespace TafraKit
{
    [SearchMenuItem("Level/Win Opened Level")]
    public class WinOpenedLevelTestModule : ActionOnInputTestingModule
    {
        protected override void OnInputReceived()
        {
            ProgressManager.WinOpenedLevel();
        }
    }
}