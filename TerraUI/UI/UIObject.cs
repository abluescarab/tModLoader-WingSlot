using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using System;
using Terraria.GameInput;

namespace TerraUI {
    public class UIObject {
        protected bool acceptsKeyboardInput = false;

        /// <summary>
        /// Fires when the object is clicked with the left button. Return true if click handled;
        /// return false to perform default left click.
        /// </summary>
        public event ClickHandler LeftClick;
        /// <summary>
        /// Fires when the object is clicked with the middle button. Return true if click handled;
        /// return false to perform default middle click.
        /// </summary>
        public event ClickHandler MiddleClick;
        /// <summary>
        /// Fires when the object is clicked with the right button. Return true if click handled;
        /// return false to perform default right click.
        /// </summary>
        public event ClickHandler RightClick;
        /// <summary>
        /// Fires when the object is clicked with the XButton1 button. Return true if click handled;
        /// return false to perform default XButton1 click.
        /// </summary>
        public event ClickHandler XButton1Click;
        /// <summary>
        /// Fires when the object is clicked with the XButton2 button. Return true if click handled;
        /// return false to perform default XButton2 click.
        /// </summary>
        public event ClickHandler XButton2Click;
        /// <summary>
        /// Fires when the object loses focus.
        /// </summary>
        public event FocusHandler LostFocus;
        /// <summary>
        /// Fires when the object gains focus.
        /// </summary>
        public event FocusHandler GotFocus;
        /// <summary>
        /// The X and Y position of the object on the screen.
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// The width and height of the object on the screen.
        /// </summary>
        public Vector2 Size { get; set; }
        /// <summary>
        /// The rectangle used to draw the object.
        /// </summary>
        public Rectangle Rectangle { get; protected set; }
        /// <summary>
        /// Whether the object has focus.
        /// </summary>
        public bool Focused { get; protected set; }
        /// <summary>
        /// The children of the object.
        /// </summary>
        public List<UIObject> Children { get; set; }
        /// <summary>
        /// The parent of the object.
        /// </summary>
        public UIObject Parent { get; set; }

        /// <summary>
        /// Create a new UIObject.
        /// </summary>
        /// <param name="position">position of the object in pixels</param>
        /// <param name="size">size of the object in pixels</param>
        /// <param name="parent">parent UIObject</param>
        /// <param name="acceptsKeyboardInput">whether the object should capture keyboard input</param>
        public UIObject(Vector2 position, Vector2 size, UIObject parent = null, bool acceptsKeyboardInput = false) {
            Position = position;
            Size = size;
            Children = new List<UIObject>();
            Parent = parent;
            this.acceptsKeyboardInput = acceptsKeyboardInput;

            if(parent != null) {
                parent.Children.Add(this);
            }
        }

        /// <summary>
        /// Update the object. Call during any PreUpdate() function.
        /// </summary>
        public virtual void Update() {
            if(!PlayerInput.IgnoreMouseInterface) {
                if(MouseUtils.Rectangle.Intersects(Rectangle)) {
                    Main.player[Main.myPlayer].mouseInterface = true;
                    Handle();
                }
                else {
                    if(MouseUtils.AnyButtonPressed()) {
                        Unfocus();
                    }
                }
            }

            foreach(UIObject obj in Children) {
                obj.Update();
            }
        }
        

        /// <summary>
        /// Handle the mouse click events.
        /// </summary>
        public void Handle() {
            if(MouseUtils.JustPressed(MouseButtons.Left)) {
                if(LeftClick == null || !LeftClick(this, new ClickEventArgs(MouseUtils.Position))) {
                    Focus();
                    DefaultLeftClick();
                }
            }
            else if(MouseUtils.JustPressed(MouseButtons.Middle)) {
                if(MiddleClick == null || !MiddleClick(this, new ClickEventArgs(MouseUtils.Position))) {
                    Focus();
                    DefaultMiddleClick();
                }
            }
            else if(MouseUtils.JustPressed(MouseButtons.Right)) {
                if(RightClick == null || !RightClick(this, new ClickEventArgs(MouseUtils.Position))) {
                    Focus();
                    DefaultRightClick();
                }
            }
            else if(MouseUtils.JustPressed(MouseButtons.XButton1)) {
                if(XButton1Click == null || !XButton1Click(this, new ClickEventArgs(MouseUtils.Position))) {
                    Focus();
                    DefaultXButton1Click();
                }
            }
            else if(MouseUtils.JustPressed(MouseButtons.XButton2)) {
                if(XButton2Click == null || !XButton2Click(this, new ClickEventArgs(MouseUtils.Position))) {
                    Focus();
                    DefaultXButton2Click();
                }
            }
        }

        /// <summary>
        /// The default left click event.
        /// </summary>
        protected virtual void DefaultLeftClick() { }
        /// <summary>
        /// The default middle click event.
        /// </summary>
        protected virtual void DefaultMiddleClick() { }
        /// <summary>
        /// The default right click event.
        /// </summary>
        protected virtual void DefaultRightClick() { }
        /// <summary>
        /// The default XButton1 click event.
        /// </summary>
        protected virtual void DefaultXButton1Click() { }
        /// <summary>
        /// The default XButton2 click event.
        /// </summary>
        protected virtual void DefaultXButton2Click() { }

        /// <summary>
        /// Draw the object. Call during any Draw() function.
        /// </summary>
        /// <param name="spriteBatch">drawing SpriteBatch</param>
        public virtual void Draw(SpriteBatch spriteBatch) {
            foreach(UIObject obj in Children) {
                obj.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Give the object focus.
        /// </summary>
        public virtual void Focus() {
            if(!Focused) {
                Focused = true;
                if(acceptsKeyboardInput) {
                    Main.blockInput = true;
                }

                if(GotFocus != null) {
                    GotFocus(this);
                }
            }
        }

        /// <summary>
        /// Take focus from the object.
        /// </summary>
        protected virtual void Unfocus() {
            if(Focused) {
                Focused = false;
                if(acceptsKeyboardInput) {
                    Main.blockInput = false;
                }

                if(LostFocus != null) {
                    LostFocus(this);
                }
            }
        }
    }
}
