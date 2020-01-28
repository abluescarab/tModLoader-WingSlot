using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace WingSlot {
    public class WingSlotConfig : ModConfig {
        public enum Location {
            Accessories,
            Uniques,
            Custom
        }

        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static WingSlotConfig Instance;

        [DefaultValue(false)]
        [Label("Allow equipping in accessory slots")]
        public bool AllowAccessorySlots;

        [DefaultValue(Location.Accessories)]
        [Label("Slot location")]
        [DrawTicks]
        public Location SlotLocation;

        [DefaultValue(false)]
        [Tooltip("Show the draggable panel for a custom location")]
        [Label("Show custom location panel")]
        public bool ShowCustomLocationPanel;

        public override void OnChanged() {
            WingSlot wingSlotMod = (WingSlot)mod;

            if(wingSlotMod.WingSlotUI == null) return;

            wingSlotMod.WingSlotUI.Panel.Visible = ShowCustomLocationPanel;
            wingSlotMod.WingSlotUI.Panel.CanDrag = (SlotLocation == Location.Custom);
        }
    }
}
