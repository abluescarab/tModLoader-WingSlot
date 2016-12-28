using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerraUI;
using TerraUI.Objects;
using TerraUI.Utilities;

namespace WingSlot {
    internal class WingSlotPlayer : ModPlayer {
        private const string HIDDEN_TAG = "hidden";
        private const string WINGS_TAG = "wings";
        private const string VANITY_WINGS_TAG = "vanitywings";
        private const string WING_DYE_TAG = "wingdye";
        private const string WING_DYE_LAYER = "WingDye";
        private PlayerLayer wingsDye;

        public UIItemSlot EquipWingSlot;
        public UIItemSlot VanityWingSlot;
        public UIItemSlot WingDyeSlot;

        public override bool Autoload(ref string name) {
            return true;
        }

        public override void Initialize() {
            EquipWingSlot = new UIItemSlot(Vector2.Zero, context: Contexts.EquipAccessory,
                conditions: Slot_Conditions, drawBackground: Slot_DrawBackground,
                scaleToInventory: true);
            VanityWingSlot = new UIItemSlot(Vector2.Zero, context: Contexts.EquipAccessoryVanity,
                conditions: Slot_Conditions, drawBackground: Slot_DrawBackground,
                scaleToInventory: true);
            WingDyeSlot = new UIItemSlot(Vector2.Zero, context: Contexts.EquipDye,
                conditions: WingDyeSlot_Conditions, drawBackground: WingDyeSlot_DrawBackground,
                scaleToInventory: true);
            VanityWingSlot.Partner = EquipWingSlot;

            // Big thanks to thegamemaster1234 for the example code used to write this!
            wingsDye = new PlayerLayer(UIUtils.Mod.Name, WING_DYE_LAYER, delegate (PlayerDrawInfo drawInfo) {
                Player player = drawInfo.drawPlayer;
                WingSlotPlayer wsp = player.GetModPlayer<WingSlotPlayer>(UIUtils.Mod);
                Item wings = wsp.GetDyedWings();
                Item dye = wsp.WingDyeSlot.Item;
                int index = Main.playerDrawData.Count - 1;
                
                if(dye.stack <= 0 || wings.stack <= 0 || !wings.active || wings.noUseGraphic || player.mount.Active)
                    return;

                if(wings.flame)
                    index -= 1;

                if(index < 0 || index > Main.playerDrawData.Count)
                    return;

                DrawData data = Main.playerDrawData[index];
                data.shader = dye.dye;
                Main.playerDrawData[index] = data;
            });

            InitializeWings();
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

        private void WingDyeSlot_DrawBackground(UIObject sender, SpriteBatch spriteBatch) {
            UIItemSlot slot = (UIItemSlot)sender;

            if(ShouldDrawSlots()) {
                slot.OnDrawBackground(spriteBatch);

                if(slot.Item.stack == 0) {
                    Texture2D tex = Main.extraTexture[54];
                    Rectangle rectangle = tex.Frame(3, 6, 1 % 3, 1 / 3);
                    rectangle.Width -= 2;
                    rectangle.Height -= 2;
                    Vector2 origin = rectangle.Size() / 2f * Main.inventoryScale;
                    Vector2 position = slot.Rectangle.TopLeft();

                    spriteBatch.Draw(
                        tex,
                        position + (slot.Rectangle.Size() / 2f) - (origin / 2f),
                        new Rectangle?(rectangle),
                        Color.White * 0.35f,
                        0f,
                        origin,
                        Main.inventoryScale,
                        SpriteEffects.None,
                        0f); // layer depth 0 = front
                }
            }
        }

        private bool WingDyeSlot_Conditions(Item item) {
            if(item.dye > 0 && item.hairDye < 0) {
                return true;
            }
            return false;
        }

        public override void PreUpdate() {
            if(ShouldDrawSlots()) {
                EquipWingSlot.Update();
                VanityWingSlot.Update();
                WingDyeSlot.Update();
            }

            UIUtils.UpdateInput();

            base.PreUpdate();
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers) {
            if(!Main.gameMenu) {
                layers.Insert(layers.IndexOf(PlayerLayer.Wings) + 1, wingsDye);
            }
        }

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
                { HIDDEN_TAG, EquipWingSlot.ItemVisible },
                { WINGS_TAG, ItemIO.Save(EquipWingSlot.Item) },
                { VANITY_WINGS_TAG, ItemIO.Save(VanityWingSlot.Item) },
                { WING_DYE_TAG, ItemIO.Save(WingDyeSlot.Item) }
            };
        }

        public override void Load(TagCompound tag) {
            SetWings(false, ItemIO.Load(tag.GetCompound(WINGS_TAG)));
            SetWings(true, ItemIO.Load(tag.GetCompound(VANITY_WINGS_TAG)));
            SetDye(ItemIO.Load(tag.GetCompound(WING_DYE_TAG)));
            EquipWingSlot.ItemVisible = tag.GetBool(HIDDEN_TAG);
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

        /// <summary>
        /// Initialize the items in the UIItemSlots.
        /// </summary>
        private void InitializeWings() {
            EquipWingSlot.Item = new Item();
            VanityWingSlot.Item = new Item();
            WingDyeSlot.Item = new Item();
            EquipWingSlot.Item.SetDefaults();
            VanityWingSlot.Item.SetDefaults();
            WingDyeSlot.Item.SetDefaults();
        }

        /// <summary>
        /// Set the item in the specified slot.
        /// </summary>
        /// <param name="isVanity">whether to equip in the vanity slot</param>
        /// <param name="item">wings</param>
        public void SetWings(bool isVanity, Item item) {
            if(!isVanity) {
                EquipWingSlot.Item = item.Clone();
            }
            else {
                VanityWingSlot.Item = item.Clone();
            }
        }

        /// <summary>
        /// Clear the wings from the specified slot.
        /// </summary>
        /// <param name="isVanity">whether to unequip from the vanity slot</param>
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

        /// <summary>
        /// Set the wing dye.
        /// </summary>
        /// <param name="item">dye</param>
        public void SetDye(Item item) {
            WingDyeSlot.Item = item.Clone();
        }

        /// <summary>
        /// Clear the wing dye.
        /// </summary>
        public void ClearDye() {
            WingDyeSlot.Item = new Item();
            WingDyeSlot.Item.SetDefaults();
        }

        /// <summary>
        /// Equip a set of wings.
        /// </summary>
        /// <param name="isVanity">whether the wings should go in the vanity slot</param>
        /// <param name="item">wings</param>
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

        /// <summary>
        /// Equip a dye.
        /// </summary>
        /// <param name="item">dye to equip</param>
        public void EquipDye(Item item) {
            int fromSlot = Array.FindIndex(player.inventory, i => i == item);

            // from inv to slot
            if(fromSlot > -1) {
                item.favorited = false;
                player.inventory[fromSlot] = WingDyeSlot.Item.Clone();
                UIUtils.PlaySound(Sounds.Grab);
                Recipe.FindRecipes();
                SetDye(item);
            }
        }

        /// <summary>
        /// Get the set of wings that a dye should be applied to.
        /// </summary>
        /// <returns>dyed wings</returns>
        public Item GetDyedWings() {
            if(VanityWingSlot.Item.stack > 0) {
                return VanityWingSlot.Item;
            }
            else if(EquipWingSlot.Item.stack > 0) {
                return EquipWingSlot.Item;
            }

            return new Item();
        }
    }
}
