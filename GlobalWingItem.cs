using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using TerraUI;
using Microsoft.Xna.Framework.Input;

namespace WingSlot {
    class GlobalWingItem : GlobalItem {
        public override bool Autoload(ref string name) {
            return true;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            DrawWingSlots(spriteBatch);
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
                    mp.SwapWings(true, item);
                }
                else {
                    mp.SwapWings(false, item);
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
        /// <param name="spriteBatch"></param>
        private void DrawWingSlots(SpriteBatch spriteBatch) {
            if(Main.playerInventory && Main.EquipPage == 2) {
                Texture2D backTex = Main.inventoryBackTexture;
                Texture2D tick = Main.inventoryTickOnTexture;

                Rectangle slotRect = new Rectangle(0, 0, (int)(backTex.Width * Main.inventoryScale), (int)(backTex.Height * Main.inventoryScale));
                
                WingSlotPlayer mp = Main.player[Main.myPlayer].GetModPlayer<WingSlotPlayer>(mod);

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
                
                //if(Main.netMode == 1) {
                //    rX -= 47;
                //}

                slotRect.X = rX;
                slotRect.Y = rY;

                mp.EquipWingSlot.Position = new Vector2(slotRect.X, slotRect.Y);
                mp.VanityWingSlot.Position = new Vector2(slotRect.X - 47, slotRect.Y);

                mp.VanityWingSlot.Draw(spriteBatch);
                mp.EquipWingSlot.Draw(spriteBatch);
                
                Main.inventoryScale = origScale;
            }
        }
    }
}
