using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace WingSlot {
    public class WingSlotConfig : ModConfig {
        public enum Location {
            Accessories,
            Custom
        }

        private Location lastSlotLocation = Location.Accessories;

        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static WingSlotConfig Instance;

        [Header("$Mods.WingSlot.SlotLocation_Header")]
        [DefaultValue(Location.Accessories)]
        [Label("$Mods.WingSlot.SlotLocation_Label")]
        [DrawTicks]
        public Location SlotLocation;

        [DefaultValue(false)]
        [Tooltip("$Mods.WingSlot.ShowCustomLocationPanel_Tooltip")]
        [Label("$Mods.WingSlot.ShowCustomLocationPanel_Label")]
        public bool ShowCustomLocationPanel;

        [DefaultValue(false)]
        [Tooltip("$Mods.WingSlot.ResetCustomSlotLocation_Tooltip")]
        [Label("$Mods.WingSlot.ResetCustomSlotLocation_Label")]
        public bool ResetCustomSlotLocation;

        public override void OnChanged() {
            if(WingSlotSystem.UI == null)
                return;

            if(lastSlotLocation == Location.Custom && SlotLocation != Location.Custom)
                ShowCustomLocationPanel = false;

            if(ShowCustomLocationPanel)
                SlotLocation = Location.Custom;

            WingSlotSystem.UI.Panel.Visible = ShowCustomLocationPanel;
            WingSlotSystem.UI.Panel.CanDrag = ShowCustomLocationPanel;

            if(ResetCustomSlotLocation) {
                WingSlotSystem.UI.ResetPosition();
                ResetCustomSlotLocation = false;
            }

            lastSlotLocation = SlotLocation;
        }
    }
}
