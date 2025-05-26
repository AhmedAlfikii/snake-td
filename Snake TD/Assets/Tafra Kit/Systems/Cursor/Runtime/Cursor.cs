using UnityEngine;
using TafraKit.Internal.Cursor;
using UnityEngine.UI;

namespace TafraKit.Cursor
{
    public static class Cursor
    {
        private static UICursor customCursor;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            CursorSettings settings = TafraSettings.GetSettings<CursorSettings>();

            if(settings == null || !settings.Enable)
                return;

            if(settings.HideOSCursor)
            {
                #if UNITY_EDITOR
                if (!settings.EditorForceShowOSCursor)
                    UnityEngine.Cursor.visible = false;
                #else
                UnityEngine.Cursor.visible = false;
                #endif
            }

            if(settings.CustomCursor != null)
            {
                GameObject cursorCanvasGO = new GameObject("Cursor Canvas");

                Canvas cursorCanvas = cursorCanvasGO.AddComponent<Canvas>();
                cursorCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                cursorCanvas.sortingOrder = 32767;

                CanvasScaler cursorCanvasScaler = cursorCanvasGO.AddComponent<CanvasScaler>();
                cursorCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                cursorCanvasScaler.referenceResolution = new Vector2(1080, 1920);
                cursorCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

                customCursor = GameObject.Instantiate(settings.CustomCursor, cursorCanvasGO.transform);

                GameObject.DontDestroyOnLoad(cursorCanvasGO);
            }
        }
    }
}