using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WingSlot {
    public class WingSlotPlayer : ModPlayer {
        private const string PanelXTag = "panelx";
        private const string PanelYTag = "panely";

        public override void SaveData(TagCompound tag) {
            tag.Add(PanelXTag, WingSlot.UI.Panel.Left.Pixels);
            tag.Add(PanelYTag, WingSlot.UI.Panel.Top.Pixels);
        }

        public override void LoadData(TagCompound tag) {
            if(tag.ContainsKey(PanelXTag))
                WingSlot.UI.Panel.Left.Set(tag.GetFloat(PanelXTag), 0);

            if(tag.ContainsKey(PanelYTag))
                WingSlot.UI.Panel.Top.Set(tag.GetFloat(PanelYTag), 0);
        }
    }
}
