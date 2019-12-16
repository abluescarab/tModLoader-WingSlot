using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

/*
 * TODO: fix number text
 * TODO: equip/unequip
 * TODO: partner slot (right-clicking vanity crashes game)
 */

namespace CustomSlot {
    public class CustomItemSlot : UIElement, ICustomSlot {
        public enum ArmorType {
            HeadArmor,
            ChestArmor,
            LegArmor
        }

        internal const int TickOffsetX = 3;
        internal const int TickOffsetY = 2;

        private readonly SlotInterior interior;
        private ToggleVisibilityButton toggleButton;
        private bool forceToggleButton;

        public bool ItemVisible { get; set; }

        public string HoverText { get; set; }

        public int Context => interior.Context;

        public CroppedTexture2D BackgroundTexture {
            get => interior.BackgroundTexture;
            set {
                interior.BackgroundTexture = value;
                CalculateSize(this, TickOffsetX, TickOffsetY);
            }
        }

        public CroppedTexture2D EmptyTexture {
            get => interior.EmptyTexture;
            set => interior.EmptyTexture = value;
        }

        public Item Item {
            get => interior.Item;
            set => interior.Item = value;
        }

        public Func<Item, bool> IsValidItem {
            get => interior.IsValidItem;
            set => interior.IsValidItem = value;
        }

        public float Scale {
            get => interior.Scale;
            set {
                interior.Scale = value;
                CalculateSize(this, TickOffsetX, TickOffsetY);
            }
        }

        public bool ForceToggleButton {
            get => forceToggleButton;
            set {
                forceToggleButton = value;
                bool hasButton = ForceToggleButton || HasToggleButton(Context);

                if(!hasButton) {
                    if(toggleButton == null) return;

                    RemoveChild(toggleButton);
                    toggleButton = null;
                }
                else {
                    toggleButton = new ToggleVisibilityButton();
                    Append(toggleButton);
                }
            }
        }

        public CustomItemSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f,
            ArmorType defaultArmorIcon = ArmorType.HeadArmor) {
            Texture2D backgroundTexture = GetBackgroundTexture(context);

            interior = new SlotInterior(
                context,
                scale,
                null,
                new CroppedTexture2D(backgroundTexture),
                GetEmptyTexture(context, defaultArmorIcon));

            Append(interior);

            ItemVisible = true;
            ForceToggleButton = false;

            CalculateSize(this, TickOffsetX, TickOffsetY);
        }

        internal static void CalculateSize(ICustomSlot slot, int offsetX, int offsetY) {
            if(slot.BackgroundTexture == CroppedTexture2D.Empty) return;

            float width = (slot.BackgroundTexture.Texture.Width * slot.Scale) + offsetX;
            float height = (slot.BackgroundTexture.Texture.Height * slot.Scale) + offsetY;

            UIElement element = (UIElement)slot;
            element.Width.Set(width, 0f);
            element.Height.Set(height, 0f);
        }

        internal class SlotInterior : UIElement, ICustomSlot {
            private Item item;
            private CroppedTexture2D backgroundTexture;
            private float scale;

            public int Context { get; }
            public Func<Item, bool> IsValidItem { get; set; }
            public CroppedTexture2D EmptyTexture { get; set; }

            public Item Item {
                get => item;
                set => item = value;
            }

            public CroppedTexture2D BackgroundTexture {
                get => backgroundTexture;
                set {
                    backgroundTexture = value;
                    CalculateSize(this, 0, 0);
                }
            }

            public float Scale {
                get => scale;
                set {
                    scale = value;
                    CalculateSize(this, 0, 0);
                }
            }

            internal SlotInterior(int context, float scale, Func<Item, bool> isValidItem,
                CroppedTexture2D backgroundTexture, CroppedTexture2D emptyTexture) {
                Context = context;
                Scale = scale;
                IsValidItem = isValidItem;
                BackgroundTexture = backgroundTexture;
                EmptyTexture = emptyTexture;
                Item = new Item();
                Item.SetDefaults();
            }

            protected override void DrawSelf(SpriteBatch spriteBatch) {
                DoDraw(spriteBatch);

                if(ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface) {
                    Main.LocalPlayer.mouseInterface = true;

                    CustomItemSlot parent = (CustomItemSlot)Parent;

                    if(parent.toggleButton != null && parent.toggleButton.ContainsPoint(Main.MouseScreen)) return;

                    if(IsValidItem == null || IsValidItem(Main.mouseItem) || Main.mouseItem.type == 0) {
                        ItemSlot.Handle(ref item, Context);

                        if(!string.IsNullOrEmpty(parent.HoverText)) {
                            Main.hoverItemName = parent.HoverText;
                        }
                    }
                }
            }

            private void DoDraw(SpriteBatch spriteBatch) {
                Rectangle parentRectangle = Parent.GetDimensions().ToRectangle();
                Rectangle rectangle = GetDimensions().ToRectangle();
                Texture2D itemTexture = EmptyTexture.Texture;
                Rectangle itemRectangle = EmptyTexture.Rectangle;
                Color color = Color.White * 0.35f;

                if(Item.stack > 0) {
                    itemTexture = Main.itemTexture[Item.type];
                    itemRectangle = Main.itemAnimations[Item.type] != null
                        ? Main.itemAnimations[Item.type].GetFrame(itemTexture) : itemTexture.Frame();
                    color = Color.White;
                }

                spriteBatch.Draw(
                    BackgroundTexture.Texture,
                    parentRectangle.TopLeft() + new Vector2(0, TickOffsetY),
                    BackgroundTexture.Rectangle,
                    Color.White * 0.8f,
                    0f,
                    Vector2.Zero,
                    Scale,
                    SpriteEffects.None,
                    1f);

                spriteBatch.Draw(
                    itemTexture,
                    rectangle.Center(),
                    itemRectangle,
                    color,
                    0f,
                    new Vector2(itemRectangle.Center.X - itemRectangle.Location.X,
                                itemRectangle.Center.Y - itemRectangle.Location.Y - TickOffsetY),
                    Scale,
                    SpriteEffects.None,
                    0f);

                // TODO: fix number text
                //ChatManager.DrawColorCodedStringWithShadow(
                //    spriteBatch,
                //    Main.fontItemStack,
                //    Item.stack.ToString(),
                //    position + new Vector2(10f, 26f) * Scale,
                //    Color.White,
                //    0f,
                //    Vector2.Zero,
                //    new Vector2(Scale),
                //    -1f,
                //    Scale);
            }
        }

        internal class ToggleVisibilityButton : UIElement {
            internal ToggleVisibilityButton() {
                Width.Set(Main.inventoryTickOnTexture.Width, 0f);
                Height.Set(Main.inventoryTickOnTexture.Height, 0f);
            }

            protected override void DrawSelf(SpriteBatch spriteBatch) {
                if(!(Parent is CustomItemSlot slot)) return;

                DoDraw(spriteBatch, slot);

                if(ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface) {
                    Main.LocalPlayer.mouseInterface = true;
                    Main.hoverItemName = Language.GetTextValue(slot.ItemVisible ? "LegacyInterface.59" : "LegacyInterface.60");

                    if(Main.mouseLeftRelease && Main.mouseLeft) {
                        Main.PlaySound(SoundID.MenuTick);
                        slot.ItemVisible = !slot.ItemVisible;
                    }
                }
            }

            private void DoDraw(SpriteBatch spriteBatch, CustomItemSlot slot) {
                Rectangle parentRectangle = Parent.GetDimensions().ToRectangle();
                Texture2D tickTexture =
                    slot.ItemVisible ? Main.inventoryTickOnTexture : Main.inventoryTickOffTexture;

                Left.Set(parentRectangle.Width - Width.Pixels + TickOffsetX, 0f);

                spriteBatch.Draw(
                    tickTexture,
                    new Vector2(parentRectangle.Right - tickTexture.Width + TickOffsetX, parentRectangle.Top),
                    Color.White * 0.7f);
            }
        }

        /// <summary>
        /// Get the background texture of a slot based on its context.
        /// </summary>
        /// <param name="context">slot context</param>
        /// <returns>background texture of the slot</returns>
        public static Texture2D GetBackgroundTexture(int context) {
            switch(context) {
                case ItemSlot.Context.EquipAccessory:
                case ItemSlot.Context.EquipArmor:
                case ItemSlot.Context.EquipGrapple:
                case ItemSlot.Context.EquipMount:
                case ItemSlot.Context.EquipMinecart:
                case ItemSlot.Context.EquipPet:
                case ItemSlot.Context.EquipLight:
                    return Main.inventoryBack3Texture;
                case ItemSlot.Context.EquipArmorVanity:
                case ItemSlot.Context.EquipAccessoryVanity:
                    return Main.inventoryBack8Texture;
                case ItemSlot.Context.EquipDye:
                    return Main.inventoryBack12Texture;
                case ItemSlot.Context.ChestItem:
                    return Main.inventoryBack5Texture;
                case ItemSlot.Context.BankItem:
                    return Main.inventoryBack2Texture;
                case ItemSlot.Context.GuideItem:
                case ItemSlot.Context.PrefixItem:
                case ItemSlot.Context.CraftingMaterial:
                    return Main.inventoryBack4Texture;
                case ItemSlot.Context.TrashItem:
                    return Main.inventoryBack7Texture;
                case ItemSlot.Context.ShopItem:
                    return Main.inventoryBack6Texture;
                default:
                    return Main.inventoryBackTexture;
            }
        }

        /// <summary>
        /// Get the empty texture of a slot based on its context.
        /// </summary>
        /// <param name="context">slot context</param>
        /// <param name="armorType">type of equipment in the slot</param>
        /// <returns>empty texture of the slot</returns>
        public static CroppedTexture2D GetEmptyTexture(int context, ArmorType armorType = ArmorType.HeadArmor) {
            int frame = -1;

            switch(context) {
                case ItemSlot.Context.EquipArmor:
                    switch(armorType) {
                        case ArmorType.HeadArmor:
                            frame = 0;
                            break;
                        case ArmorType.ChestArmor:
                            frame = 6;
                            break;
                        case ArmorType.LegArmor:
                            frame = 12;
                            break;
                    }
                    break;
                case ItemSlot.Context.EquipArmorVanity:
                    switch(armorType) {
                        case ArmorType.HeadArmor:
                            frame = 3;
                            break;
                        case ArmorType.ChestArmor:
                            frame = 9;
                            break;
                        case ArmorType.LegArmor:
                            frame = 15;
                            break;
                    }
                    break;
                case ItemSlot.Context.EquipAccessory:
                    frame = 11;
                    break;
                case ItemSlot.Context.EquipAccessoryVanity:
                    frame = 2;
                    break;
                case ItemSlot.Context.EquipDye:
                    frame = 1;
                    break;
                case ItemSlot.Context.EquipGrapple:
                    frame = 4;
                    break;
                case ItemSlot.Context.EquipMount:
                    frame = 13;
                    break;
                case ItemSlot.Context.EquipMinecart:
                    frame = 7;
                    break;
                case ItemSlot.Context.EquipPet:
                    frame = 10;
                    break;
                case ItemSlot.Context.EquipLight:
                    frame = 17;
                    break;
            }

            if(frame == -1) return new CroppedTexture2D();

            Texture2D extraTextures = Main.extraTexture[54];
            Rectangle rectangle = extraTextures.Frame(3, 6, frame % 3, frame / 3);
            rectangle.Width -= 2;
            rectangle.Height -= 2;

            return new CroppedTexture2D(extraTextures, rectangle);
        }

        /// <summary>
        /// Whether the slot has a visibility toggle button.
        /// </summary>
        public static bool HasToggleButton(int context) {
            return context == ItemSlot.Context.EquipAccessory ||
                   context == ItemSlot.Context.EquipLight ||
                   context == ItemSlot.Context.EquipPet;
        }
    }
}
