using System.ComponentModel;
using CustomSlot.UI;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;

namespace WingSlot {
    public class WingSlotConfig : ModConfig {

        private AccessorySlotsUI.Location lastSlotLocation = AccessorySlotsUI.Location.Accessories;

        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static WingSlotConfig Instance;

        [DefaultValue(false)]
        [Label("$Mods.WingSlot.AllowAccessorySlots_Label")]
        public bool AllowAccessorySlots;

        [Header("$Mods.WingSlot.SlotLocation_Header")]
        [DefaultValue(AccessorySlotsUI.Location.Accessories)]
        [Label("$Mods.WingSlot.SlotLocation_Label")]
        [DrawTicks]
        public AccessorySlotsUI.Location SlotLocation;

        [DefaultValue(false)]
        [Tooltip("$Mods.WingSlot.ShowCustomLocationPanel_Tooltip")]
        [Label("$Mods.WingSlot.ShowCustomLocationPanel_Label")]
        public bool ShowCustomLocationPanel;

        [DefaultValue(false)] 
        [Tooltip("$Mods.WingSlot.ResetCustomSlotLocation_Tooltip")]
        [Label("$Mods.WingSlot.ResetCustomSlotLocation_Label")]
        public bool ResetCustomSlotLocation;

        public override void OnChanged() {
            if(WingSlot.UI == null) return;

            if(lastSlotLocation == AccessorySlotsUI.Location.Custom && SlotLocation != AccessorySlotsUI.Location.Custom) {
                ShowCustomLocationPanel = false;
            }

            WingSlot.UI.Panel.Visible = ShowCustomLocationPanel;
            WingSlot.UI.Panel.CanDrag = ShowCustomLocationPanel;

            if(ShowCustomLocationPanel) {
                SlotLocation = AccessorySlotsUI.Location.Custom;
            }

            if(SlotLocation == AccessorySlotsUI.Location.Custom) {
                WingSlot.UI.MoveToCustomPosition();
            }

            lastSlotLocation = SlotLocation;
            WingSlot.UI.PanelLocation = SlotLocation;

            if(ResetCustomSlotLocation) {
                WingSlot.UI.ResetPosition();
                ResetCustomSlotLocation = false;
            }
        }
    }
}
