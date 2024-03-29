﻿using CustomSlot;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WingSlot {
    public class WingSlotPlayer : ModPlayer {
        private PlayerData<float> panelX = new("panelX", 0f);
        private PlayerData<float> panelY = new("panelY", 0f);
        private PlayerData<bool> firstLoad = new("firstLoad", true);

        public override void SaveData(TagCompound tag) {
            tag.Add(panelX.Tag, WingSlotSystem.UI.Panel.Left.Pixels);
            tag.Add(panelY.Tag, WingSlotSystem.UI.Panel.Top.Pixels);
            tag.Add(firstLoad.Tag, false);
        }

        public override void LoadData(TagCompound tag) {
            if(tag.GetBool(firstLoad.Tag)) {
                Vector2 defaultPos = WingSlotSystem.UI.DefaultCoordinates;
                panelX.Value = defaultPos.X;
                panelY.Value = defaultPos.Y;
            }
            else {
                panelX.Value = tag.GetFloat(panelX.Tag);
                panelY.Value = tag.GetFloat(panelY.Tag);
            }
        }

        public override void OnEnterWorld() {
            WingSlotSystem.UI.Panel.Visible =
                ModContent
                .GetInstance<WingSlotConfig>()
                .ShowCustomLocationPanel;
            WingSlotSystem.UI.Panel.Left.Set(panelX.Value, 0);
            WingSlotSystem.UI.Panel.Top.Set(panelY.Value, 0);
        }
    }
}
