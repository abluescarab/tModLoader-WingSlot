using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerraUI;

namespace WingSlot {
    internal class WingSlotPlayer : ModPlayer {
        private const ushort installed = 0;

        public Item Wings;
        public Item VanityWings;
        public UIItemSlot UIWingSlot;
        public UIItemSlot VanityWingSlot;

        public override bool Autoload(ref string name) {
            return true;
        }

        public override void Initialize() {
            InitializeWings();

            UIWingSlot = new UIItemSlot(Vector2.Zero, context: Contexts.EquipAccessory);
            VanityWingSlot = new UIItemSlot(Vector2.Zero, context: Contexts.EquipAccessoryVanity);

            UIWingSlot.Conditions = VanityWingSlot.Conditions =
                delegate (Item item) {
                    if(item.wingSlot > 0) {
                        return true;
                    }
                    return false;
                };

            UIWingSlot.DrawItem = VanityWingSlot.DrawItem =
                delegate (SpriteBatch spriteBatch, UIItemSlot slot) {
                    if(slot.Item.stack > 0 && Main.EquipPage == 2) {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                        Texture2D tex = Main.itemTexture[slot.Item.type];
                        Vector2 origin = new Vector2(
                            tex.Width / 2 * Main.inventoryScale,
                            tex.Height / 2 * Main.inventoryScale);

                        spriteBatch.Draw(
                            tex,
                            new Vector2(
                                slot.Rectangle.X + (slot.Rectangle.Width / 2) - (origin.X / 2),
                                slot.Rectangle.Y + (slot.Rectangle.Height / 2) - (origin.Y / 2)),
                            null,
                            Color.White,
                            0f,
                            origin,
                            Main.inventoryScale,
                            SpriteEffects.None,
                            0f);

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    }
                };

            UIWingSlot.DrawBackground = VanityWingSlot.DrawBackground =
                delegate (SpriteBatch spriteBatch, UIItemSlot slot) {
                    if(Main.EquipPage == 2) {
                        Texture2D backTexture = UIUtils.GetContextTexture(slot.Context);

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                        spriteBatch.Draw(
                            backTexture,
                            new Vector2(slot.Rectangle.X, slot.Rectangle.Y),
                            null,
                            Color.White * 0.35f, // half required
                            0f,
                            Vector2.Zero,
                            Main.inventoryScale,
                            SpriteEffects.None,
                            1f); // layer depth 1 = back

                        if(slot.Item.stack == 0) {
                            Texture2D tex = mod.GetTexture(WingSlot.wingSlotBackground);
                            Vector2 origin = new Vector2(
                                tex.Width / 2 * Main.inventoryScale,
                                tex.Height / 2 * Main.inventoryScale);
                            Vector2 position = new Vector2(
                                slot.Rectangle.X + (slot.Rectangle.Width / 2) - (origin.X / 2),
                                slot.Rectangle.Y + (slot.Rectangle.Height / 2) - (origin.Y / 2));

                            spriteBatch.Draw(
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

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    }
                };

            UIWingSlot.RightClick += UIWingSlot_RightClick;
            VanityWingSlot.RightClick += VanityWingSlot_RightClick;
        }

        private bool UIWingSlot_RightClick(UIObject sender, ClickEventArgs e) {
            if(Main.EquipPage == 2) {
                UIItemSlot slot = (UIItemSlot)sender;

                if(slot.Item.stack > 0) {
                    SwapWings(false, slot.Item);
                    return true;
                }
            }

            return false;
        }

        private bool VanityWingSlot_RightClick(UIObject sender, ClickEventArgs e) {
            UIItemSlot slot = (UIItemSlot)sender;

            if(slot.Item.stack > 0) {
                SwapWings(true, slot.Item);
                return true;
            }

            return false;
        }

        private void InitializeWings() {
            Wings = new Item();
            VanityWings = new Item();
            Wings.SetDefaults();
            VanityWings.SetDefaults();
        }

        public override void PreUpdate() {
            if(Main.EquipPage == 2) {
                VanityWingSlot.Update();
                UIWingSlot.Update();
                UIUtils.UpdateInput();
            }
            base.PreUpdate();
        }

        public void SetWings(bool isVanity, Item item) {
            if(!isVanity) {
                UIWingSlot.Item = Wings = item.Clone();
            }
            else {
                VanityWingSlot.Item = VanityWings = item.Clone();
            }
        }

        public void ClearWings(bool isVanity) {
            if(!isVanity) {
                Wings = new Item();
                Wings.SetDefaults();

                UIWingSlot.Item = new Item();
                UIWingSlot.Item.SetDefaults();
            }
            else {
                VanityWings = new Item();
                VanityWings.SetDefaults();

                VanityWingSlot.Item = new Item();
                VanityWingSlot.Item.SetDefaults();
            }
        }

        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff) {
            if(Wings.stack > 0) {
                player.VanillaUpdateEquip(Wings);
                player.VanillaUpdateAccessory(Wings, !UIWingSlot.ItemVisible, ref wallSpeedBuff, ref tileSpeedBuff,
                    ref tileRangeBuff);
            }

            if(VanityWings.stack > 0) {
                player.VanillaUpdateVanityAccessory(VanityWings);
            }
        }

        public override void SaveCustomData(BinaryWriter writer) {
            int hide = (UIWingSlot.ItemVisible ? 0 : 1);

            writer.Write(installed);
            writer.Write(hide);
            WriteWings(Wings, writer);
            WriteWings(VanityWings, writer);
        }

        public override void LoadCustomData(BinaryReader reader) {
            int hide = 0;

            InitializeWings();

            ushort installedFlag = reader.ReadUInt16();

            if(installedFlag == 0) {
                Item wings = Wings;
                Item vanityWings = VanityWings;

                try { hide = reader.ReadInt32(); }
                catch(EndOfStreamException) { hide = 0; }

                UIWingSlot.ItemVisible = (hide == 1 ? false : true);

                ReadWings(ref wings, reader);
                SetWings(false, wings);
                ReadWings(ref vanityWings, reader);
                SetWings(true, vanityWings);
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
            try { ItemIO.ReadItem(wings, reader, false, false); }
            catch(EndOfStreamException) { }
        }

        public bool SwapWings(bool isVanity, Item item) {
            UIItemSlot slot = UIWingSlot;

            if(isVanity) {
                slot = VanityWingSlot;
            }

            int fromSlot = Array.FindIndex(player.inventory, i => i == item);

            // from inv to slot
            if(fromSlot > -1) {
                item.favorited = false;
                player.inventory[fromSlot] = slot.Item.Clone();
                UIUtils.PlaySound(Sounds.Grab);
                Recipe.FindRecipes();
                SetWings(isVanity, item);
            }
            // from slot to inv
            else if(item == slot.Item) {
                int toSlot = Array.FindIndex(player.inventory, 10, i => i.stack == 0);

                if(toSlot > -1 && toSlot < 50) {
                    ClearWings(isVanity);
                    UIUtils.PlaySound(Sounds.Grab);
                    Recipe.FindRecipes();
                    player.inventory[toSlot] = item.Clone();
                }
            }

            return false;
        }
    }
}
