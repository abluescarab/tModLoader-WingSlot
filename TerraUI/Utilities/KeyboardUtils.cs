using Microsoft.Xna.Framework.Input;

namespace TerraUI.Utilities {
    public static class KeyboardUtils {
        private static KeyboardState lastState;
        private static KeyboardState state;

        /// <summary>
        /// The current keyboard state.
        /// </summary>
        public static KeyboardState State {
            get { return state; }
        }

        /// <summary>
        /// The keyboard state the last time Update() was called.
        /// </summary>
        public static KeyboardState LastState {
            get { return lastState; }
        }

        /// <summary>
        /// Update the State and LastState variables.
        /// </summary>
        internal static void UpdateState() {
            lastState = state;
            state = Keyboard.GetState();
        }

        /// <summary>
        /// Check if a key was just pressed.
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>whether key was just pressed</returns>
        public static bool JustPressed(Keys key) {
            if(lastState.IsKeyUp(key) && state.IsKeyDown(key)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if a key was just released.
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>whether key was just released</returns>
        public static bool JustReleased(Keys key) {
            if(lastState.IsKeyDown(key) && state.IsKeyUp(key)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if a key is held down.
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>whether key is held down</returns>
        public static bool HeldDown(Keys key) {
            if(lastState.IsKeyDown(key) && state.IsKeyDown(key)) {
                return true;
            }

            return false;
        }
    }
}
