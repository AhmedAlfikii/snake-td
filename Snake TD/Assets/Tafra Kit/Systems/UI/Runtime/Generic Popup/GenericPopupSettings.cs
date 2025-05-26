using TafraKit.Internal.UI;

namespace TafraKit.Internal
{
    public class GenericPopupSettings : SettingsModule
    {
        public bool Enabled;
        public GenericPopupObject[] BasicPopups;

        public override int Priority => 12;
        public override string Name => "UI/Generic Popup";
        public override string Description => "Display generic popups with options.";
    }
}