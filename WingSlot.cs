using Microsoft.Xna.Framework.Graphics;
using ModConfiguration;
using Terraria;
using Terraria.ModLoader;

namespace WingSlot {
    public class WingSlot : Mod {
        public const string ALLOW_ACCESSORY_SLOTS = "allowWingsInAccessorySlots";
        public const string SLOT_LOCATION = "slotLocation";
        public const string WING_SLOT_BACK_TEX = "WingSlotBackground";
        public static readonly ModConfig Config = new ModConfig("WingSlot");

        public override void Load() {
            Properties = new ModProperties() {
                Autoload = true,
                AutoloadBackgrounds = true,
                AutoloadSounds = true
            };

            TerraUI.Utilities.UIUtils.Mod = this;
            TerraUI.Utilities.UIUtils.Subdirectory = "TerraUI";
            
            Config.Add(ALLOW_ACCESSORY_SLOTS, false);
            Config.Add(SLOT_LOCATION, 1);
            Config.Load();
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch) {
            WingSlotPlayer wsp = Main.player[Main.myPlayer].GetModPlayer<WingSlotPlayer>(this);
            wsp.Draw(spriteBatch);
        }
    }
}
