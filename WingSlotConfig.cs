using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace WingSlot {
    public class WingSlotConfig : ModConfig {
        public enum Location {
            [Label("$Mods.WingSlot.DefaultLocation_Label")]
            Default,
            [Label("$Mods.WingSlot.CustomLocation_Label")]
            Custom
        }

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.WingSlot.SlotLocation_Header")]
        [DefaultValue(Location.Default)]
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

        [Header("$Mods.WingSlot.OtherOptions_Header")]
        [DefaultValue(false)]
        [Label("$Mods.WingSlot.AllowEquipping_Label")]
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
