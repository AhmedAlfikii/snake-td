using TafraKit.Healthies;
using TafraKit.SceneManagement;
using UnityEngine;

namespace TafraKit
{
    [SearchMenuItem("Scene/Reload Scene")]
    public class SceneReloadTestModule : ActionOnInputTestingModule
    {
        protected override void OnInputReceived()
        {
            TafraSceneManager.ReloadOpenedScene();
        }
    }
}