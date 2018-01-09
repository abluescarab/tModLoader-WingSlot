using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TerraUI.Objects;

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

        /// <summary>
        /// Initialize the ModPlayer.
        /// </summary>
        public override void Initialize() {
            EquipWingSlot = new UIItemSlot(Vector2.Zero, context: ItemSlot.Context.EquipAccessory, hoverText: "Wings",
                conditions: Slot_Conditions, drawBackground: Slot_DrawBackground, scaleToInventory: true);
            VanityWingSlot = new UIItemSlot(Vector2.Zero, context: ItemSlot.Context.EquipAccessoryVanity, hoverText:
                Language.GetTextValue("LegacyInterface.11") + " Wings",
                conditions: Slot_Conditions, drawBackground: Slot_DrawBackground, scaleToInventory: true);
            WingDyeSlot = new UIItemSlot(Vector2.Zero, context: ItemSlot.Context.EquipDye, conditions: WingDyeSlot_Conditions,
                drawBackground: WingDyeSlot_DrawBackground, scaleToInventory: true);
            VanityWingSlot.Partner = EquipWingSlot;
            EquipWingSlot.BackOpacity = VanityWingSlot.BackOpacity = WingDyeSlot.BackOpacity = .8f;

            // Big thanks to thegamemaster1234 for the example code used to write this!
            wingsDye = new PlayerLayer(mod.Name, WING_DYE_LAYER, delegate (PlayerDrawInfo drawInfo) {
                Player player = drawInfo.drawPlayer;
                WingSlotPlayer wsp = player.GetModPlayer<WingSlotPlayer>(mod);
                Item wings = wsp.GetDyedWings();
                Item dye = wsp.WingDyeSlot.Item;

                if(dye.stack <= 0 || wings.stack <= 0 || !wings.active || wings.noUseGraphic || player.mount.Active ||
                  (wsp.VanityWingSlot.Item.stack <= 0 && !wsp.EquipWingSlot.ItemVisible && player.wingFrame == 0))
                    return;

                int shader = GameShaders.Armor.GetShaderIdFromItemId(dye.type);

                for(int i = 0; i < Main.playerDrawData.Count; i++) {
                    DrawData data = Main.playerDrawData[i];
                    data.shader = shader;
                    Main.playerDrawData[i] = data;
                }
            });

            InitializeWings();
        }

        /// <summary>
        /// Modify draw layers to draw the wing dye.
        /// </summary>
        /// <param name="layers"></param>
        public override void ModifyDrawLayers(List<PlayerLayer> layers) {
            if(!Main.gameMenu) {
                layers.Insert(layers.IndexOf(PlayerLayer.Wings) + 1, wingsDye);
            }
        }

        /// <summary>
        /// Update player with the equipped wings.
        /// </summary>
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

        /// <summary>
        /// Save the mod settings.
        /// </summary>
        public override TagCompound Save() {
            return new TagCompound {
                { HIDDEN_TAG, EquipWingSlot.ItemVisible },
                { WINGS_TAG, ItemIO.Save(EquipWingSlot.Item) },
                { VANITY_WINGS_TAG, ItemIO.Save(VanityWingSlot.Item) },
                { WING_DYE_TAG, ItemIO.Save(WingDyeSlot.Item) }
            };
        }

        /// <summary>
        /// Load the mod settings.
        /// </summary>
        public override void Load(TagCompound tag) {
            SetWings(false, ItemIO.Load(tag.GetCompound(WINGS_TAG)));
            SetWings(true, ItemIO.Load(tag.GetCompound(VANITY_WINGS_TAG)));
            SetDye(ItemIO.Load(tag.GetCompound(WING_DYE_TAG)));
            EquipWingSlot.ItemVisible = tag.GetBool(HIDDEN_TAG);
        }

        /// <summary>
        /// Load legacy mod settings.
        /// </summary>
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

                if(context == ItemSlot.Context.EquipAccessory) {
                    SetWings(false, wings1);
                    SetWings(true, wings2);
                }
                else if(context == ItemSlot.Context.EquipAccessoryVanity) {
                    SetWings(true, wings1);
                    SetWings(false, wings2);
                }
            }
        }

        /// <summary>
        /// Read the wings in legacy mod settings.
        /// </summary>
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
        /// Draw the wing slot backgrounds.
        /// </summary>
        private void Slot_DrawBackground(UIObject sender, SpriteBatch spriteBatch) {
            UIItemSlot slot = (UIItemSlot)sender;

            if(ShouldDrawSlots()) {
                slot.OnDrawBackground(spriteBatch);

                if(slot.Item.stack == 0) {
                    Texture2D tex = mod.GetTexture(WingSlot.WING_SLOT_BACK_TEX);
                    Vector2 origin = tex.Size() / 2f * Main.inventoryScale;
                    Vector2 position = slot.Rectangle.TopLeft();

                    spriteBatch.Draw(
                        tex,
                        position + (slot.Rectangle.Size() / 2f) - (origin / 2f),
                        null,
                        Color.White * 0.35f,
                        0f,
                        origin,
                        Main.inventoryScale,
                        SpriteEffects.None,
                        0f); // layer depth 0 = front
                }
            }
        }

        /// <summary>
        /// Control what can be placed in the wing slots.
        /// </summary>
        private bool Slot_Conditions(Item item) {
            if(item.wingSlot > 0) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draw the wing dye slot background.
        /// </summary>
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

        /// <summary>
        /// Control what can be placed in the wing dye slot.
        /// </summary>
        private bool WingDyeSlot_Conditions(Item item) {
            if(item.dye > 0 && item.hairDye < 0) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draw the wing slots.
        /// </summary>
        /// <param name="spriteBatch">drawing SpriteBatch</param>
        public void Draw(SpriteBatch spriteBatch) {
            if(ShouldDrawSlots()) {
                int mapH = 0;
                int rX = 0;
                int rY = 0;
                float origScale = Main.inventoryScale;

                Main.inventoryScale = 0.85f;

                if(Main.mapEnabled) {
                    int adjustY = 600;

                    if(!Main.mapFullscreen && Main.mapStyle == 1) {
                        mapH = 256;
                    }

                    if(Main.player[Main.myPlayer].ExtraAccessorySlotsShouldShow) {
                        adjustY = 610 + PlayerInput.UsingGamepad.ToInt() * 30;
                    }

                    if((mapH + adjustY) > Main.screenHeight) {
                        mapH = Main.screenHeight - adjustY;
                    }
                }

                int slotCount = 7 + Main.player[Main.myPlayer].extraAccessorySlots;

                if((Main.screenHeight < 900) && (slotCount >= 8)) {
                    slotCount = 7;
                }

                rX = Main.screenWidth - 92 - 14 - (47 * 3) - (int)(Main.extraTexture[58].Width * Main.inventoryScale);
                rY = (int)(mapH + 174 + 4 + slotCount * 56 * Main.inventoryScale);

                EquipWingSlot.Position = new Vector2(rX, rY);
                VanityWingSlot.Position = new Vector2(rX -= 47, rY);
                WingDyeSlot.Position = new Vector2(rX -= 47, rY);

                VanityWingSlot.Draw(spriteBatch);
                EquipWingSlot.Draw(spriteBatch);
                WingDyeSlot.Draw(spriteBatch);

                Main.inventoryScale = origScale;

                EquipWingSlot.Update();
                VanityWingSlot.Update();
                WingDyeSlot.Update();
            }
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
                Main.PlaySound(SoundID.Grab);
                Recipe.FindRecipes();
                SetWings(isVanity, item);
            }
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
                Main.PlaySound(SoundID.Grab);
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
