using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
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
        
        public UIItemSlot EquipWingSlot;
        public UIItemSlot VanityWingSlot;
        //public UIItemSlot WingDyeSlot;

        public override bool Autoload(ref string name) {
            return true;
        }

        public override void Initialize() {
            EquipWingSlot = new UIItemSlot(Vector2.Zero, context: Contexts.EquipAccessory);
            VanityWingSlot = new UIItemSlot(Vector2.Zero, context: Contexts.EquipAccessoryVanity);
            //WingDyeSlot = new UIItemSlot(Vector2.Zero, context: Contexts.EquipDye);
            VanityWingSlot.Partner = EquipWingSlot;
            EquipWingSlot.ScaleToInventory = true;
            VanityWingSlot.ScaleToInventory = true;
            //WingDyeSlot.ScaleToInventory = true;
            
            InitializeWings();
            
            EquipWingSlot.DrawBackground += Slot_DrawBackground;
            EquipWingSlot.Conditions += Slot_Conditions;
            
            VanityWingSlot.DrawBackground += Slot_DrawBackground;
            VanityWingSlot.Conditions += Slot_Conditions;

            //WingDyeSlot.DrawBackground += WingDyeSlot_DrawBackground;
            //WingDyeSlot.Conditions += WingDyeSlot_Conditions;
        }

        private void Slot_DrawBackground(UIObject sender, SpriteBatch spriteBatch) {
            UIItemSlot slot = (UIItemSlot)sender;

            if(ShouldDrawSlots()) {
                slot.OnDrawBackground(spriteBatch);

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
            }
        }

        private bool Slot_Conditions(Item item) {
            if(item.wingSlot > 0) {
                return true;
            }
            return false;
        }

        //private void WingDyeSlot_DrawBackground(UIObject sender, SpriteBatch spriteBatch) {
        //    UIItemSlot slot = (UIItemSlot)sender;

        //    if(ShouldDraw()) {
        //        slot.OnDrawBackground(spriteBatch);

        //        if(slot.Item.stack == 0) {
        //            Texture2D tex = Main.extraTexture[54];
        //            Rectangle rectangle = tex.Frame(3, 6, 1 % 3, 1 / 3);
        //            rectangle.Width -= 2;
        //            rectangle.Height -= 2;
        //            Vector2 origin = rectangle.Size() / 2f * Main.inventoryScale;
        //            Vector2 position = slot.Rectangle.TopLeft();

        //            spriteBatch.Draw(
        //                tex,
        //                position + (slot.Rectangle.Size() / 2f) - (origin / 2f),
        //                new Rectangle?(rectangle),
        //                Color.White * 0.35f,
        //                0f,
        //                origin,
        //                Main.inventoryScale,
        //                SpriteEffects.None,
        //                0f); // layer depth 0 = front
        //        }
        //    }
        //}

        //private bool WingDyeSlot_Conditions(Item item) {
        //    if(item.dye > 0 && item.hairDye < 0) {
        //        return true;
        //    }
        //    return false;
        //}

        public override void PreUpdate() {
            if(ShouldDrawSlots()) {
                EquipWingSlot.Update();
                VanityWingSlot.Update();
                //WingDyeSlot.Update();
            }

            UIUtils.UpdateInput();

            base.PreUpdate();
        }

        private void InitializeWings() {
            EquipWingSlot.Item = new Item();
            VanityWingSlot.Item = new Item();
            //WingDyeSlot.Item = new Item();
            EquipWingSlot.Item.SetDefaults();
            VanityWingSlot.Item.SetDefaults();
            //WingDyeSlot.Item.SetDefaults();
        }

        public void SetWings(bool isVanity, Item item) {
            if(!isVanity) {
                EquipWingSlot.Item = item.Clone();
            }
            else {
                VanityWingSlot.Item = item.Clone();
            }
        }

        public void ClearWings(bool isVanity) {
            if(!isVanity) {
                EquipWingSlot.Item = new Item();
                EquipWingSlot.Item.SetDefaults();
            }
            else {
                VanityWingSlot.Item = new Item();
                VanityWingSlot.Item.SetDefaults();
            }
        }

        //public void SetDye(Item item) {
        //    WingDyeSlot.Item = item.Clone();
        //}

        //public void ClearDye() {
        //    WingDyeSlot.Item = new Item();
        //    WingDyeSlot.Item.SetDefaults();
        //}

        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff) {
            Item wings = EquipWingSlot.Item;
            Item vanityWings = VanityWingSlot.Item;

            if(wings.stack > 0) {
                player.VanillaUpdateEquip(wings);
                player.VanillaUpdateAccessory(player.whoAmI, wings, !EquipWingSlot.ItemVisible, ref wallSpeedBuff, ref tileSpeedBuff,
                    ref tileRangeBuff);
            }

            if(vanityWings.stack > 0) {
                player.VanillaUpdateVanityAccessory(vanityWings);
            }
        }

        public override TagCompound Save() {
            return new TagCompound {
                { HIDDEN, EquipWingSlot.ItemVisible },
                { WINGS, ItemIO.Save(EquipWingSlot.Item) },
                { VANITY_WINGS, ItemIO.Save(VanityWingSlot.Item) } //,
                //{ WING_DYE, ItemIO.Save(WingDyeSlot.Item) }
            };
        }

        public override void Load(TagCompound tag) {
            SetWings(false, ItemIO.Load(tag.GetCompound(WINGS)));
            SetWings(true, ItemIO.Load(tag.GetCompound(VANITY_WINGS)));
            //SetDye(ItemIO.Load(tag.GetCompound(WING_DYE)));
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

                Item wings1 = EquipWingSlot.Item;
                Item wings2 = VanityWingSlot.Item;

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

        public void EquipWings(bool isVanity, Item item) {
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
        }

        public bool ShouldDrawSlots() {
            if(Main.playerInventory && Main.EquipPage == 2) {
                return true;
            }

            return false;
        }

        //public void EquipDye(Item item) {
        //    int fromSlot = Array.FindIndex(player.inventory, i => i == item);

        //    // from inv to slot
        //    if(fromSlot > -1) {
        //        item.favorited = false;
        //        player.inventory[fromSlot] = WingDyeSlot.Item.Clone();
        //        UIUtils.PlaySound(Sounds.Grab);
        //        Recipe.FindRecipes();
        //        SetDye(item);
        //    }
        //}
    }
}
