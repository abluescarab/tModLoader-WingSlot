using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;

namespace TerraUI {
    public static class UIUtils {
        public static Mod Mod { get; set; }

        public static void PlaySound(Sounds type, int x = -1, int y = -1, int style = 1) {
            Main.PlaySound((int)type, x, y, style);
        }

        /// <summary>
        /// Updates the current mouse and keyboard state.
        /// Call after all UIObjects have been updated.
        /// </summary>
        public static void UpdateInput() {
            MouseUtils.UpdateState();
            KeyboardUtils.UpdateState();
        }

        public static ButtonState GetButtonState(MouseButtons mouseButton, MouseState mouseState) {
            switch(mouseButton) {
                case MouseButtons.Left:
                    return mouseState.LeftButton;
                case MouseButtons.Middle:
                    return mouseState.MiddleButton;
                case MouseButtons.Right:
                    return mouseState.RightButton;
                case MouseButtons.XButton1:
                    return mouseState.XButton1;
                case MouseButtons.XButton2:
                    return mouseState.XButton2;
                default:
                    return ButtonState.Released;
            }
        }

        public static Texture2D GetContextTexture(Contexts context) {
            switch(context) {
                case Contexts.EquipAccessory:
                case Contexts.EquipArmor:
                case Contexts.EquipGrapple:
                case Contexts.EquipMount:
                case Contexts.EquipMinecart:
                case Contexts.EquipPet:
                case Contexts.EquipLight:
                    return Main.inventoryBack3Texture;
                case Contexts.EquipArmorVanity:
                case Contexts.EquipAccessoryVanity:
                    return Main.inventoryBack8Texture;
                case Contexts.EquipDye:
                    return Main.inventoryBack12Texture;
                case Contexts.ChestItem:
                    return Main.inventoryBack5Texture;
                case Contexts.BankItem:
                    return Main.inventoryBack2Texture;
                case Contexts.GuideItem:
                case Contexts.PrefixItem:
                case Contexts.CraftingMaterial:
                    return Main.inventoryBack4Texture;
                case Contexts.TrashItem:
                    return Main.inventoryBack7Texture;
                case Contexts.ShopItem:
                    return Main.inventoryBack6Texture;
                default:
                    return Main.inventoryBackTexture;
            }
        }
    }
}
