using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;

namespace UtilitySlots {
    public static class KeyboardUtils {
        /// <summary>
        /// Whether the shift key is pressed.
        /// </summary>
        public static bool Shift => Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift);
        /// <summary>
        /// Whether the control key is pressed.
        /// </summary>
        public static bool Control => Main.keyState.IsKeyDown(Keys.LeftControl) || Main.keyState.IsKeyDown(Keys.RightControl);
        /// <summary>
        /// Whether the alt key is pressed.
        /// </summary>
        public static bool Alt => Main.keyState.IsKeyDown(Keys.LeftAlt) || Main.keyState.IsKeyDown(Keys.RightAlt);

        /// <summary>
        /// Check if a key was just pressed.
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>whether key was just pressed</returns>
        public static bool JustPressed(Keys key) {
            return Main.oldKeyState.IsKeyUp(key) && Main.keyState.IsKeyDown(key);
        }

        /// <summary>
        /// Check if a key was just released.
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>whether key was just released</returns>
        public static bool JustReleased(Keys key) {
            return Main.oldKeyState.IsKeyDown(key) && Main.keyState.IsKeyUp(key);
        }

        /// <summary>
        /// Check if a key is held down.
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>whether key is held down</returns>
        public static bool HeldDown(Keys key) {
            return Main.oldKeyState.IsKeyDown(key) && Main.keyState.IsKeyDown(key);
        }
    }
}
