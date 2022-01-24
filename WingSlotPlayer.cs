using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WingSlot {
    public class WingSlotPlayer : ModPlayer {
        private const string PanelXTag = "panelx";
        private const string PanelYTag = "panely";

        public override void SaveData(TagCompound tag) {
            tag.Add(PanelXTag, WingSlotSystem.UI.Panel.Left.Pixels);
            tag.Add(PanelYTag, WingSlotSystem.UI.Panel.Top.Pixels);
        }

        public override void LoadData(TagCompound tag) {
            if(tag.ContainsKey(PanelXTag))
                WingSlotSystem.UI.Panel.Left.Set(tag.GetFloat(PanelXTag), 0);

            if(tag.ContainsKey(PanelYTag))
                WingSlotSystem.UI.Panel.Top.Set(tag.GetFloat(PanelYTag), 0);
        }
    }
}
