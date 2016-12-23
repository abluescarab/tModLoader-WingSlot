using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerraUI;
using TerraUI.Objects;
using TerraUI.Utilities;

namespace WingSlot {
    internal class WingSlotPlayer : ModPlayer {
        private const ushort installed = 0;
        private const string HIDDEN = "hidden";
        private const string WINGS = "wings";
        private const string VANITY_WINGS = "vanitywings";
        //private const string WING_DYE = "wingdye";

        public Item Wings;
        public Item VanityWings;
        //public Item WingDye;
        public UIItemSlot EquipWingSlot;
        public UIItemSlot VanityWingSlot;
        //public UIItemSlot WingDyeSlot;

        public override bool Autoload(ref string name) {
            return true;
        }

        public override void Initialize() {
            InitializeWings();

            EquipWingSlot = new UIItemSlot(Vector2.Zero, context: Contexts.EquipAccessory);
            VanityWingSlot = new UIItemSlot(Vector2.Zero, context: Contexts.EquipAccessoryVanity);
            //WingDyeSlot = new UIItemSlot(Vector2.Zero, context: Contexts.EquipDye);

            //WingDyeSlot.DrawAsNormalItemSlot = true;

            EquipWingSlot.Conditions = VanityWingSlot.Conditions =
                delegate (Item item) {
                    if(item.wingSlot > 0) {
                        return true;
                    }
                    return false;
                };

            EquipWingSlot.DrawItem = VanityWingSlot.DrawItem += new DrawHandler(delegate (UIObject sender, SpriteBatch spriteBatch) {
                UIItemSlot slot = (UIItemSlot)sender;

                if(slot.Item.stack > 0 && Main.EquipPage == 2) {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                    Texture2D tex = Main.itemTexture[slot.Item.type];
                    Vector2 origin = tex.Size() / 2f * Main.inventoryScale;
                    Vector2 position = slot.Rectangle.TopLeft();

                    spriteBatch.Draw(
                        tex,
                        position + (slot.Rectangle.Size() / 2f) - (origin / 2f),
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
            });

            EquipWingSlot.DrawBackground = VanityWingSlot.DrawBackground += new DrawHandler(
                delegate (UIObject sender, SpriteBatch spriteBatch) {
                    UIItemSlot slot = (UIItemSlot)sender;

                    if(Main.EquipPage == 2) {
                        Texture2D backTexture = UIUtils.GetContextTexture(slot.Context);

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                        spriteBatch.Draw(
                            backTexture,
                            slot.Rectangle.TopLeft(),
                            null,
                            Color.White * 0.35f,
                            0f,
                            Vector2.Zero,
                            Main.inventoryScale,
                            SpriteEffects.None,
                            1f); // layer depth 1 = back

                        if(slot.Item.stack == 0) {
                            Texture2D tex = mod.GetTexture(WingSlot.wingSlotBackground);
                            Vector2 origin = tex.Size() / 2f * Main.inventoryScale;
                            Vector2 position = slot.Rectangle.TopLeft();
                            
                            spriteBatch.Draw(
                                tex,
                                position + (slot.Rectangle.Size() / 2f) - (origin / 2f),
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
                });

            //WingDyeSlot.Conditions = delegate (Item item) {
            //    if(item.dye > 0) {
            //        return true;
            //    }
            //    return false;
            //};

            EquipWingSlot.Click += EquipWingSlot_Click;
            VanityWingSlot.Click += VanityWingSlot_Click;
        }

        private bool VanityWingSlot_Click(UIObject sender, MouseButtonEventArgs e) {
            UIItemSlot slot = (UIItemSlot)sender;

            if(e.Button == MouseButtons.Left) {
                if(slot.Item.stack > 0) {
                    slot.DefaultLeftClick();
                    ClearWings(true);
                    return true;
                }
            }
            else if(e.Button == MouseButtons.Right) {
                if(slot.Item.stack > 0) {
                    SwapWings(true, slot.Item);
                    return true;
                }
            }

            return false;
        }

        private bool EquipWingSlot_Click(UIObject sender, MouseButtonEventArgs e) {
            UIItemSlot slot = (UIItemSlot)sender;

            if(e.Button == MouseButtons.Left) {
                if(slot.Item.stack > 0) {
                    slot.DefaultLeftClick();
                    ClearWings(false);
                    return true;
                }
            }
            else if(e.Button == MouseButtons.Right) {
                if(slot.Item.stack > 0) {
                    SwapWings(false, slot.Item);
                    return true;
                }
            }

            return false;
        }

        public override void PreUpdate() {
            if(Main.EquipPage == 2) {
                EquipWingSlot.Update();
                VanityWingSlot.Update();
                //WingDyeSlot.Update();
            }

            UIUtils.UpdateInput();

            base.PreUpdate();
        }

        private void InitializeWings() {
            Wings = new Item();
            VanityWings = new Item();
            //WingDye = new Item();
            Wings.SetDefaults();
            VanityWings.SetDefaults();
            //WingDye.SetDefaults();
        }

        public void SetWings(bool isVanity, Item item) {
            if(!isVanity) {
                EquipWingSlot.Item = Wings = item.Clone();
            }
            else {
                VanityWingSlot.Item = VanityWings = item.Clone();
            }
        }

        public void ClearWings(bool isVanity) {
            if(!isVanity) {
                Wings = new Item();
                Wings.SetDefaults();

                EquipWingSlot.Item = new Item();
                EquipWingSlot.Item.SetDefaults();
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
                player.VanillaUpdateAccessory(player.whoAmI, Wings, !EquipWingSlot.ItemVisible, ref wallSpeedBuff, ref tileSpeedBuff,
                    ref tileRangeBuff);
            }

            if(VanityWings.stack > 0) {
                player.VanillaUpdateVanityAccessory(VanityWings);
            }
        }

        public override TagCompound Save() {
            return new TagCompound {
                { HIDDEN, EquipWingSlot.ItemVisible },
                { WINGS, ItemIO.Save(Wings) },
                { VANITY_WINGS, ItemIO.Save(VanityWings) } //,
                //{ WING_DYE, ItemIO.Save(WingDye) }
            };
        }

        public override void Load(TagCompound tag) {
            SetWings(false, ItemIO.Load(tag.GetCompound(WINGS)));
            SetWings(true, ItemIO.Load(tag.GetCompound(VANITY_WINGS)));
            //WingDye = ItemIO.Load(tag.GetCompound(WING_DYE));
            EquipWingSlot.ItemVisible = tag.GetBool(HIDDEN);
        }

        public override void LoadLegacy(BinaryReader reader) {
            int hide = 0;

            InitializeWings();

            ushort installedFlag = reader.ReadUInt16();

            if(installedFlag == 0) {
                try { hide = reader.ReadInt32(); }
                catch(EndOfStreamException) { hide = 0; }

                EquipWingSlot.ItemVisible = (hide == 1 ? false : true);

                Item wings1 = Wings;
                Item wings2 = VanityWings;

                int context = ReadWingsLegacy(ref wings1, reader);
                ReadWingsLegacy(ref wings2, reader);

                if(context == (int)Contexts.EquipAccessory) {
                    SetWings(false, wings1);
                    SetWings(true, wings2);
                }
                else if(context == (int)Contexts.EquipAccessoryVanity) {
                    SetWings(true, wings1);
                    SetWings(false, wings2);
                }
            }
        }

        internal static int ReadWingsLegacy(ref Item wings, BinaryReader reader) {
            try {
                ItemIO.LoadLegacy(wings, reader, false, false);
                return reader.ReadInt32();
            }
            catch(EndOfStreamException) {
                return -1;
            }
        }

        public bool SwapWings(bool isVanity, Item item) {
            UIItemSlot slot = (isVanity ? VanityWingSlot : EquipWingSlot);

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

        //public void EquipDye(Item item) {
            //WingDyeSlot.Item = WingDye = item.Clone();

            //Item invi = player.inventory.FirstOrDefault(i => i.Equals(item));

            //if(invi != null) {
            //    invi = new Item();
            //    invi.SetDefaults();
            //}
        //}
    }
}
