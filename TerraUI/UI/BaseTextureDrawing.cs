using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace TerraUI {
    public static class BaseTextureDrawing {
        public static void DrawRectangleBox(SpriteBatch sb, Color colour, Color colour2, Rectangle rect, int width) {
            Texture2D texture = UIUtils.Mod.GetTexture("Textures/1x1");

            sb.Draw(texture, new Rectangle(rect.X + width, rect.Y + width, rect.Width - (width * 2), rect.Height - (width * 2)), colour2);
            sb.Draw(texture, new Rectangle((int)(rect.X), (int)(rect.Y), rect.Width, width), new Rectangle(0, 0, 0, 0), colour);
            sb.Draw(texture, new Rectangle((int)(rect.X), (int)(rect.Y), width, rect.Height), new Rectangle(0, 0, 0, 0), colour);
            sb.Draw(texture, new Rectangle((int)(rect.X), (int)(rect.Y + rect.Height - width), rect.Width, width), new Rectangle(0, 0, 0, 0), colour);
            sb.Draw(texture, new Rectangle((int)(rect.X + rect.Width - width), (int)(rect.Y), width, rect.Height), new Rectangle(0, 0, 0, 0), colour);
        }

        public static void DrawTerrariaStyledBox(SpriteBatch sb, Color colour, Rectangle rect, bool solid = false) {
            string add = "";

            if(solid) {
                add = "Solid";
            }

            sb.Draw(UIUtils.Mod.GetTexture("Textures/Corner" + add), new Vector2(rect.X, rect.Y), colour);
            sb.Draw(UIUtils.Mod.GetTexture("Textures/Corner" + add), new Vector2(rect.X + rect.Width, rect.Y), null, colour, (float)(Math.PI / 2), default(Vector2), 1f, SpriteEffects.None, 0f);
            sb.Draw(UIUtils.Mod.GetTexture("Textures/Corner" + add), new Vector2(rect.X + rect.Width, rect.Y + rect.Height), null, colour, (float)(Math.PI), default(Vector2), 1f, SpriteEffects.None, 0f);
            sb.Draw(UIUtils.Mod.GetTexture("Textures/Corner" + add), new Vector2(rect.X, rect.Y + rect.Height), null, colour, (float)(Math.PI * 1.5), default(Vector2), 1f, SpriteEffects.None, 0f);
            sb.Draw(UIUtils.Mod.GetTexture("Textures/Side" + add), new Rectangle(rect.X + 16, rect.Y, rect.Width - 32, 16), colour);
            sb.Draw(UIUtils.Mod.GetTexture("Textures/Side" + add), new Rectangle(rect.X + rect.Width, rect.Y + 16, rect.Height - 32, 16), null, colour, (float)(Math.PI / 2), default(Vector2), SpriteEffects.None, 0f);
            sb.Draw(UIUtils.Mod.GetTexture("Textures/Side" + add), new Rectangle(rect.X + rect.Width - 16, rect.Y + rect.Height, rect.Width - 32, 16), null, colour, (float)(Math.PI), default(Vector2), SpriteEffects.None, 0f);
            sb.Draw(UIUtils.Mod.GetTexture("Textures/Side" + add), new Rectangle(rect.X, rect.Y + rect.Height - 16, rect.Height - 32, 16), null, colour, (float)(Math.PI * 1.5), default(Vector2), SpriteEffects.None, 0f);
            sb.Draw(UIUtils.Mod.GetTexture("Textures/Background" + add), new Rectangle(rect.X + 16, rect.Y + 16, rect.Width - 32, rect.Height - 32), null, colour);
        }
    }
}
