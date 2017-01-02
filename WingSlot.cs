using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace WingSlot {
    public class WingSlot : Mod {
        public const string WING_SLOT_BACK_TEX = "WingSlotBackground";

        public override void Load() {
            Properties = new ModProperties() {
                Autoload = true,
                AutoloadBackgrounds = true,
                AutoloadSounds = true
            };

            TerraUI.Utilities.UIUtils.Mod = this;
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch) {
            WingSlotPlayer wsp = Main.player[Main.myPlayer].GetModPlayer<WingSlotPlayer>(this);
            wsp.Draw(spriteBatch);
        }

        /// <summary>
        /// Whether to draw the UIItemSlots.
        /// </summary>
        /// <returns>whether to draw the slots</returns>
        public bool ShouldDrawSlots() {
            if(Main.playerInventory && Main.EquipPage == 2) {
                return true;
            }

            return false;
        }
    }
}
