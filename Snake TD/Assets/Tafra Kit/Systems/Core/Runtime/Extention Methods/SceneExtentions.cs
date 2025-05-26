using UnityEngine;
using UnityEngine.SceneManagement;

namespace TafraKit
{
    public static class SceneExtentions
    {
        public static bool WasSceneLoadedAdditively(this Scene scene)
        {
            return scene.isLoaded && SceneManager.GetActiveScene() != scene;
        }
    }
}