using UnityEngine;
using TafraKit;
using TafraKit.UI;

namespace TafraKit.Internal.Cursor
{
    public class CursorSettings : SettingsModule
    {
        [SerializeField] private bool enable;
        [SerializeField] private bool hideOSCursor;
        [SerializeField] private UICursor customCursor;
        [SerializeField] private bool hideCustomCursorByDefault;

        [Header("Editor")]
        [SerializeField] private bool editorForceShowOSCursor;

        public bool Enable => enable;
        public bool HideOSCursor => hideOSCursor;
        public UICursor CustomCursor => customCursor;
        public bool HideCustomCursorByDefault => hideCustomCursorByDefault;
        public bool EditorForceShowOSCursor => editorForceShowOSCursor;

        public override string Name => "UI/Cursor";
        public override string Description => "Control the OS cursor.";
    }
}