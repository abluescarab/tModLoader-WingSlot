using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;
using TerraUI.Utilities;

namespace WingSlot {
    class GlobalWingItem : GlobalItem {
        public override bool Autoload(ref string name) {
            return true;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame,
            Color drawColor, Color itemColor, Vector2 origin, float scale) {
            DrawSlots(spriteBatch);
            base.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override bool CanEquipAccessory(Item item, Player player, int slot) {
            if(item.wingSlot > 0) {
                return false;
            }
            return base.CanEquipAccessory(item, player, slot);
        }

        public override bool CanRightClick(Item item) {
            if(item.wingSlot > 0) {
                return true;
            }

            return base.CanRightClick(item);
        }

        public override void RightClick(Item item, Player player) {
            if(item.wingSlot > 0) {
                WingSlotPlayer mp = player.GetModPlayer<WingSlotPlayer>(mod);

                if(KeyboardUtils.HeldDown(Keys.LeftShift)) {
                    mp.EquipWings(true, item);
                }
                else {
                    mp.EquipWings(false, item);
                }
            }
            else {
                base.RightClick(item, player);
            }
        }

        /// <summary>
        /// Draws the wing equipment slots.
        /// Based on code provided by jopojelly.
        /// </summary>
        private void DrawSlots(SpriteBatch spriteBatch) {
            WingSlotPlayer mp = Main.player[Main.myPlayer].GetModPlayer<WingSlotPlayer>(mod);

            if(mp.ShouldDrawSlots()) {

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

                rX = Main.screenWidth - 92 - (47 * 2);
                rY = mapH + 174;

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
