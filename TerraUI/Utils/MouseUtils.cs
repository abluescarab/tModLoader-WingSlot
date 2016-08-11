using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using System;

namespace TerraUI {
    public static class MouseUtils {
        private static MouseState lastState;
        private static MouseState state;
        
        /// <summary>
        /// The current mouse state.
        /// </summary>
        public static MouseState State {
            get { return state; }
        }

        /// <summary>
        /// The mouse state the last time Update() was called.
        /// </summary>
        public static MouseState LastState {
            get { return lastState; }
        }

        /// <summary>
        /// The mouse position rectangle.
        /// </summary>
        public static Rectangle Rectangle {
            get { return new Rectangle(Main.mouseX, Main.mouseY, 1, 1); }
        }

        /// <summary>
        /// The mouse position.
        /// </summary>
        public static Vector2 Position {
            get { return new Vector2(Main.mouseX, Main.mouseY); }
        }

        /// <summary>
        /// Update the State and LastState variables.
        /// </summary>
        internal static void UpdateState() {
            lastState = state;
            state = Mouse.GetState();
        }

        /// <summary>
        /// Check if a button was just pressed.
        /// </summary>
        /// <param name="mouseButton">button to check</param>
        /// <returns>whether button was just pressed</returns>
        public static bool JustPressed(MouseButtons mouseButton) {
            if(UIUtils.GetButtonState(mouseButton, lastState) == ButtonState.Released &&
               UIUtils.GetButtonState(mouseButton, state) == ButtonState.Pressed) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if a button was just released.
        /// </summary>
        /// <param name="mouseButton">button to check</param>
        /// <returns>whether button was just released</returns>
        public static bool JustReleased(MouseButtons mouseButton) {
            if(UIUtils.GetButtonState(mouseButton, lastState) == ButtonState.Pressed &&
               UIUtils.GetButtonState(mouseButton, state) == ButtonState.Released) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if a button is held down.
        /// </summary>
        /// <param name="mouseButton">button to check</param>
        /// <param name="frames">how many frames the button must be held down before returning true</param>
        /// <returns>whether button is held down</returns>
        public static bool HeldDown(MouseButtons mouseButton) {
            if(UIUtils.GetButtonState(mouseButton, lastState) == ButtonState.Pressed &&
               UIUtils.GetButtonState(mouseButton, state) == ButtonState.Pressed) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if any button has just been pressed.
        /// </summary>
        /// <returns>whether any button has just been pressed</returns>
        public static bool AnyButtonPressed() {
            foreach(MouseButtons button in Enum.GetValues(typeof(MouseButtons))) {
                if(JustPressed(button)) {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Check if any button has just been pressed.
        /// </summary>
        /// <param name="pressedButton">pressed button</param>
        /// <returns>whether any button has just been pressed</returns>
        public static bool AnyButtonPressed(out MouseButtons pressButton) {
            foreach(MouseButtons button in Enum.GetValues(typeof(MouseButtons))) {
                if(JustPressed(button)) {
                    pressButton = button;
                    return true;
                }
            }

            pressButton = MouseButtons.None;
            return false;
        }

        /// <summary>
        /// Check if any button has just been released.
        /// </summary>
        /// <returns>whether any button has just been released</returns>
        public static bool AnyButtonReleased() {
            //if(JustReleased(MouseButtons.Left))
            foreach(MouseButtons button in Enum.GetValues(typeof(MouseButtons))) {
                if(JustPressed(button)) {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Check if any button has just been released.
        /// </summary>
        /// <param name="releasedButton">released button</param>
        /// <returns>whether any button has just been released</returns>
        public static bool AnyButtonReleased(out MouseButtons releasedButton) {
            //if(JustReleased(MouseButtons.Left))
            foreach(MouseButtons button in Enum.GetValues(typeof(MouseButtons))) {
                if(JustPressed(button)) {
                    releasedButton = button;
                    return true;
                }
            }

            releasedButton = MouseButtons.None;
            return false;
        }
    }
}
