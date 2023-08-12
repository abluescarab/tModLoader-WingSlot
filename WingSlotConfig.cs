using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace WingSlot {
    public class WingSlotConfig : ModConfig {
        public enum Location {
            Default,
            Custom
        }

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("SlotLocation")]
        [DefaultValue(Location.Default)]
        [DrawTicks]
        public Location SlotLocation;

        [DefaultValue(false)]
        public bool ShowCustomLocationPanel;

        [DefaultValue(false)]
        public bool ResetCustomSlotLocation;

        [Header("OtherOptions")]
        [DefaultValue(false)]
        public bool AllowEquippingInOtherSlots;

        public override void OnChanged() {
            if(SlotLocation == Location.Default) {
                ShowCustomLocationPanel = false;
            }

            if(WingSlotSystem.UI != null) {
                WingSlotSystem.UI.Panel.Visible = ShowCustomLocationPanel;
                WingSlotSystem.UI.Panel.CanDrag = ShowCustomLocationPanel;

                if(ResetCustomSlotLocation) {
                    WingSlotSystem.UI.ResetPosition();
                    ResetCustomSlotLocation = false;
                }
            }
        }
    }
}
