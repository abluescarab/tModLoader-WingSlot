using CustomSlot.UI;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using static CustomSlot.UI.AccessorySlotsUI;

namespace WingSlot {
    public class WingSlotConfig : ModConfig {
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
            if(WingSlotSystem.UI == null) {
                return;
            }

            WingSlotSystem.UI.Panel.Visible = ShowCustomLocationPanel;
            WingSlotSystem.UI.Panel.CanDrag = ShowCustomLocationPanel;
            WingSlotSystem.UI.PanelLocation = SlotLocation;

            if(ResetCustomSlotLocation) {
                WingSlotSystem.UI.ResetPosition();
                ResetCustomSlotLocation = false;
            }
        }
    }
}
