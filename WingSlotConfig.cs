using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace WingSlot {
    public class WingSlotConfig : ModConfig {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        [Tooltip("Place the new slots next to accessories or next to\nunique equipment (pets, minecart, etc.)")]
        [Label("Slots next to accessories")]
        public bool SlotsNextToAccessories;

        [DefaultValue(false)]
        [Label("Allow equipping in accessory slots")]
        public bool AllowAccessorySlots;

        public override void OnChanged() {
            WingSlot.SlotsNextToAccessories = SlotsNextToAccessories;
            WingSlot.AllowAccessorySlots = AllowAccessorySlots;
        }
    }
}
