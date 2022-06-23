using System.ComponentModel;
using CustomSlot.UI;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;

namespace UtilitySlots {
    public class UtilitySlotsConfig : ModConfig {

        private AccessorySlotsUI.Location lastSlotLocation = AccessorySlotsUI.Location.Accessories;

        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static UtilitySlotsConfig Instance;

        [DefaultValue(false)]
        [Label("$Mods.UtilitySlot.AllowAccessorySlots_Label")]
        public bool AllowAccessorySlots;

        /*[Header("$Mods.UtilitySlot.SlotLocation_Header")]
        [DefaultValue(AccessorySlotsUI.Location.Accessories)]
        [Label("$Mods.UtilitySlot.SlotLocation_Label")]
        [DrawTicks]
        public AccessorySlotsUI.Location SlotLocation;

        [DefaultValue(false)]
        [Tooltip("$Mods.UtilitySlot.ShowCustomLocationPanel_Tooltip")]
        [Label("$Mods.UtilitySlot.ShowCustomLocationPanel_Label")]
        public bool ShowCustomLocationPanel;

        [DefaultValue(false)] 
        [Tooltip("$Mods.UtilitySlot.ResetCustomSlotLocation_Tooltip")]
        [Label("$Mods.UtilitySlot.ResetCustomSlotLocation_Label")]
        public bool ResetCustomSlotLocation;*/

        public override void OnChanged() {
            /*if(lastSlotLocation == AccessorySlotsUI.Location.Custom && SlotLocation != AccessorySlotsUI.Location.Custom) {
                ShowCustomLocationPanel = false;
            }*/

            if(UtilitySlots.WingUI != null)
            {
                UtilitySlots.WingUI.Panel.Visible = false;
                UtilitySlots.WingUI.Panel.CanDrag = false;
            }
            if(UtilitySlots.BalloonUI != null)
            {
                UtilitySlots.BalloonUI.Panel.Visible = false;
                UtilitySlots.BalloonUI.Panel.CanDrag = false;
            }
            if(UtilitySlots.ShoeUI != null)
            {
                UtilitySlots.ShoeUI.Panel.Visible = false;
                UtilitySlots.ShoeUI.Panel.CanDrag = false;
            }

            /*if(ShowCustomLocationPanel) {
                SlotLocation = AccessorySlotsUI.Location.Custom;
            }

            if(SlotLocation == AccessorySlotsUI.Location.Custom) {
                UtilitySlots.WingUI.MoveToCustomPosition();
            }

            lastSlotLocation = SlotLocation;
            UtilitySlots.WingUI.PanelLocation = SlotLocation;

            if(ResetCustomSlotLocation) {
                UtilitySlots.WingUI.ResetPosition();
                ResetCustomSlotLocation = false;
            }*/
        }
    }
}
