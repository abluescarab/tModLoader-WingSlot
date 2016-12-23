using Terraria.ModLoader;

namespace WingSlot {
    public class WingSlot : Mod {
        public const string wingSlotBackground = "WingSlotBackground";

        public override void Load() {
            Properties = new ModProperties() {
                Autoload = true,
                AutoloadBackgrounds = true,
                AutoloadSounds = true
            };

            AddGlobalItem("GlobalWingItem", new GlobalWingItem());
            //AddGlobalItem("GlobalDyeItem", new GlobalDyeItem());
        }
    }
}
