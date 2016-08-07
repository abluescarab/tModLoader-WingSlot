using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using WingSlot.UI;

namespace WingSlot {
    public class WingSlot : Mod {
        public const string wingSlotBackground = "WingSlotBackground";

        public override void Load() {
            Properties = new ModProperties() {
                Autoload = true,
                AutoloadBackgrounds = true,
                AutoloadSounds = true
            };

            AddGlobalItem("GlobalWingItem", new GlobalWingItem());
        }
    }
}
