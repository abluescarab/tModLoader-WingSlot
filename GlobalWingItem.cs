using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace WingSlot {
    class GlobalWingItem : GlobalItem {
        public override bool Autoload(ref string name) {
            return true;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            DrawWingSlot(spriteBatch);
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
                player.GetModPlayer<MPlayer>(mod).SwapWings(item);
            }
            else {
                base.RightClick(item, player);
            }
        }

        /// <summary>
        /// Draws the wing equipment slot.
        /// Based on code provided by jopojelly.
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawWingSlot(SpriteBatch spriteBatch) {
            if(Main.playerInventory && Main.EquipPage == 2) {
                Point mouse = new Point(Main.mouseX, Main.mouseY);
                Rectangle r = new Rectangle(0, 0, (int)(Main.inventoryBack3Texture.Width * Main.inventoryScale),
                    (int)(Main.inventoryBack3Texture.Height * Main.inventoryScale));
                MPlayer mp = Main.player[Main.myPlayer].GetModPlayer<MPlayer>(mod);
                int mapH = 0;
                int rX = 0;
                int rY = 0;
                float origScale = Main.inventoryScale;

                Main.inventoryScale = .85f;

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

                r.X = rX;
                r.Y = rY;

                mp.UIWingSlot.position = new Vector2(r.X, r.Y);

                if(r.Contains(mouse)) {
                    Main.armorHide = true;
                    mp.UIWingSlot.Handle();
                }
                
                mp.UIWingSlot.Draw(spriteBatch);
                Main.inventoryScale = origScale;
            }
        }
    }
}
