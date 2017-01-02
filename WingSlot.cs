using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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

            TerraUI.Utilities.UIUtils.Mod = this;
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch) {
            DrawSlots(spriteBatch);
            base.PostDrawInterface(spriteBatch);
        }

        /// <summary>
        /// Whether to draw the UIItemSlots.
        /// </summary>
        /// <returns>whether to draw the slots</returns>
        public bool ShouldDrawSlots() {
            if(Main.playerInventory && Main.EquipPage == 0) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Draws the wing equipment slots.
        /// Based on code provided by jopojelly.
        /// </summary>
        private void DrawSlots(SpriteBatch spriteBatch) {
            WingSlotPlayer mp = Main.player[Main.myPlayer].GetModPlayer<WingSlotPlayer>(this);

            if(ShouldDrawSlots()) {
                int mapH = 0;
                int rX = 0;
                int rY = 0;
                float origScale = Main.inventoryScale;

                Main.inventoryScale = 0.85f;

                if(Main.mapEnabled) {
                    if(!Main.mapFullscreen && Main.mapStyle == 1) {
                        mapH = 256;
                    }

                    if((mapH + 600) > Main.screenHeight) {
                        mapH = Main.screenHeight - 600;
                    }
                }

                rX = Main.screenWidth - 92; //- (47 * 2);
                rY = mapH + 174 + (48 * 8) + 1;

                if(Main.netMode == 1) {
                    rX -= 47;
                }

                mp.EquipWingSlot.Position = new Vector2(rX, rY);
                mp.VanityWingSlot.Position = new Vector2(rX -= 47, rY);
                mp.WingDyeSlot.Position = new Vector2(rX -= 47, rY);

                mp.VanityWingSlot.Draw(spriteBatch);
                mp.EquipWingSlot.Draw(spriteBatch);
                mp.WingDyeSlot.Draw(spriteBatch);

                Main.inventoryScale = origScale;
            }
        }
    }
}
