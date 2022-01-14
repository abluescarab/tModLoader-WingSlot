using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace WingSlot {
    public class WingSlotConfig : ModConfig {
        public enum Location {
            Accessories,
            Uniques,
            Custom
        }

        private Location lastSlotLocation = Location.Accessories;

        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static WingSlotConfig Instance;

        [DefaultValue(false)]
        [Label("$Mods.WingSlot.AllowAccessorySlots_Label")]
        public bool AllowAccessorySlots;

        [Header("$Mods.WingSlot.SlotLocation_Header")]
        [DefaultValue(Location.Accessories)]
        [Label("$Mods.WingSlot.SlotLocation_Label")]
        [DrawTicks]
        public Location SlotLocation;

        [DefaultValue(false)]
        [Tooltip("$Mods.WingSlot.ShowCustomLocationPanel_Tooltip")]
        [Label("$Mods.WingSlot.ShowCustomLocationPanel_Label")]
        public bool ShowCustomLocationPanel;

        public override void OnChanged() {
            if(WingSlot.UI == null) return;

            if(lastSlotLocation == Location.Custom && SlotLocation != Location.Custom) {
                ShowCustomLocationPanel = false;
            }

            WingSlot.UI.Panel.Visible = ShowCustomLocationPanel;
            WingSlot.UI.Panel.CanDrag = ShowCustomLocationPanel;

            if(ShowCustomLocationPanel) {
                SlotLocation = Location.Custom;
            }

            if(SlotLocation == Location.Custom) {
                WingSlot.UI.MoveToCustomPosition();
            }

            lastSlotLocation = SlotLocation;
        }
    }
}
