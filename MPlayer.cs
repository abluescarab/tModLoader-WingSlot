using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WingSlot.UI;

namespace WingSlot {
    internal class MPlayer : ModPlayer {
        private const ushort Installed = 0;
        public Item Wings;
        public bool OwnsWings = false;
        public bool HideWings = false;
        public UIItemSlot UIWingSlot;

        public override bool Autoload(ref string name) {
            return true;
        }

        public override void Initialize() {
            Wings = new Item();
            Wings.SetDefaults();

            UIWingSlot = new UIItemSlot(
                Vector2.Zero,
                condition: delegate (Item i) {
                    if(i.wingSlot > 0) {
                        return true;
                    }
                    return false;
                },
                drawBackground: delegate (SpriteBatch sb, UIItemSlot slot) {
                    if(Main.EquipPage == 2) {
                        sb.End();
                        sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                        sb.Draw(
                            Main.inventoryBack3Texture,
                            new Vector2(slot.rectangle.X, slot.rectangle.Y),
                            null,
                            Color.White * 0.35f, // half required
                            0f,
                            Vector2.Zero,
                            Main.inventoryScale,
                            SpriteEffects.None,
                            1f); // layer depth 1 = back

                        if(slot.item.stack == 0) {
                            Texture2D tex = mod.GetTexture(WingSlot.wingSlotBackground);
                            Vector2 origin = new Vector2(
                                tex.Width / 2 * Main.inventoryScale,
                                tex.Height / 2 * Main.inventoryScale);
                            Vector2 position = new Vector2(
                                slot.rectangle.X + (slot.rectangle.Width / 2) - (origin.X / 2),
                                slot.rectangle.Y + (slot.rectangle.Height / 2) - (origin.Y / 2));

                            sb.Draw(
                                tex,
                                position,
                                null,
                                Color.White * 0.15f,
                                0f,
                                origin,
                                Main.inventoryScale,
                                SpriteEffects.None,
                                0f); // layer depth 0 = front
                        }

                        sb.End();
                        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    }
                },
                drawItem: delegate (SpriteBatch sb, UIItemSlot slot) {
                    if(slot.item.stack > 0 && Main.EquipPage == 2) {
                        sb.End();
                        sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                        Texture2D tex = Main.itemTexture[slot.item.type];
                        Vector2 origin = new Vector2(
                            tex.Width / 2 * Main.inventoryScale,
                            tex.Height / 2 * Main.inventoryScale);

                        sb.Draw(
                            tex,
                            new Vector2(
                                slot.rectangle.X + (slot.rectangle.Width / 2) - (origin.X / 2),
                                slot.rectangle.Y + (slot.rectangle.Height / 2) - (origin.Y / 2)),
                            null,
                            Color.White,
                            0f,
                            origin,
                            Main.inventoryScale,
                            SpriteEffects.None,
                            0f);

                        sb.End();
                        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    }
                },
                leftClick: delegate (UIItemSlot slot) {
                    if(slot.item.stack > 0) {
                        HideWings = !HideWings;
                        return true;
                    }
                    return false;
                },
                rightClick: delegate (UIItemSlot slot) {
                    if(slot.item.stack > 0) {
                        SwapWings(slot.item);
                        return true;
                    }
                    return false;
                });
        }

        public void SetWings(Item item) {
            UIWingSlot.item = Wings = item.Clone();
            OwnsWings = true;
        }

        public void ClearWings() {
            UIWingSlot.item = Wings = new Item();
            UIWingSlot.item.SetDefaults();
            Wings.SetDefaults();
            OwnsWings = false;
        }

        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff) {
            if(!string.IsNullOrWhiteSpace(Wings.name)) {
                player.VanillaUpdateEquip(Wings);
                player.VanillaUpdateAccessory(Wings, HideWings, ref wallSpeedBuff, ref tileSpeedBuff,
                    ref tileRangeBuff);
            }
        }

        public override void SaveCustomData(BinaryWriter writer) {
            int hide = (HideWings ? 1 : 0);

            writer.Write(Installed);
            writer.Write(OwnsWings);
            writer.Write(hide);
            WriteWings(Wings, writer);
        }

        public override void LoadCustomData(BinaryReader reader) {
            int hide = 0;

            Wings = new Item();
            Wings.SetDefaults();

            ushort installedFlag = reader.ReadUInt16();

            if(installedFlag == 0) {
                Item wings = Wings;

                try {
                    OwnsWings = reader.ReadBoolean();
                }
                catch(EndOfStreamException) {
                    OwnsWings = false;
                }

                try {
                    hide = reader.ReadInt32();
                }
                catch(EndOfStreamException) {
                    hide = 0;
                }

                HideWings = (hide == 1 ? true : false);

                if(OwnsWings) {
                    ReadWings(ref wings, reader);
                    SetWings(wings);
                }
                else {
                    ClearWings();
                }
            }
        }

        internal static bool WriteWings(Item wings, BinaryWriter writer) {
            if(!string.IsNullOrWhiteSpace(wings.name)) {
                ItemIO.WriteItem(wings, writer, false, false);
                return true;
            }
            return false;
        }

        internal static void ReadWings(ref Item wings, BinaryReader reader) {
            try {
                ItemIO.ReadItem(wings, reader, false, false);
            }
            catch(EndOfStreamException) {
            }
        }
        public bool SwapWings(Item item) {
            //MPlayer mp = player.GetModPlayer<MPlayer>(mod);
            int fromSlot = Array.FindIndex(player.inventory, i => i == item);

            // from inv to slot
            if(fromSlot > -1) {
                item.favorited = false;
                player.inventory[fromSlot] = UIWingSlot.item.Clone();
                Main.PlaySound(7, -1, -1, 1);
                Recipe.FindRecipes();
                SetWings(item);
            }
            // from slot to inv
            else if(item == UIWingSlot.item) {
                int toSlot = Array.FindIndex(player.inventory, 10, i => i.stack == 0);

                if(toSlot > -1 && toSlot < 50) {
                    ClearWings();
                    Main.PlaySound(7, -1, -1, 1);
                    Recipe.FindRecipes();
                    player.inventory[toSlot] = item.Clone();
                }
            }

            return false;
        }
    }
}
