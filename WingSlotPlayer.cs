using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

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

        /// <summary>
        /// Fires when the item in a slot is changed.
        /// </summary>
        public void ItemChanged(CustomItemSlot slot, ItemChangedEventArgs e) {
            if(slot.Context == ItemSlot.Context.EquipAccessory)
                EquippedWings = e.NewItem.Clone();
            else if(slot.Context == ItemSlot.Context.EquipAccessoryVanity)
                SocialWings = e.NewItem.Clone();
            else
                WingsDye = e.NewItem.Clone();
        }

        /// <summary>
        /// Fires when the visibility of an item in a slot is toggled.
        /// </summary>
        public void ItemVisibilityChanged(CustomItemSlot slot, ItemVisibilityChangedEventArgs e) {
            WingsVisible = e.Visibility;
        }
    }
}
