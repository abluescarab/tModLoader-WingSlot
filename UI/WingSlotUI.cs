using Terraria;
using Terraria.UI;

namespace WingSlot.UI {
    public class WingSlotUI : UIState {
        public static readonly float SlotSize = 72 * Main.inventoryScale;

        /// <summary>
        /// The panel holding the item slots.
        /// </summary>
        public DraggableUIPanel Panel { get; protected set; }
        /// <summary>
        /// Whether the UI is visible or not.
        /// </summary>
        public virtual bool IsVisible => Main.playerInventory && WingSlotConfig.Instance.ShowCustomLocationPanel;

        public override void OnInitialize() {
            Panel = new DraggableUIPanel();
            Panel.Width.Set((SlotSize * 3) + 6 + Panel.PaddingLeft + Panel.PaddingRight, 0);
            Panel.Height.Set(SlotSize + Panel.PaddingTop + Panel.PaddingBottom, 0);

            ResetPosition();
            Append(Panel);
        }

        public void ResetPosition() {
            Panel.Left.Set(Main.screenWidth / 2.0f, 0);
            Panel.Top.Set(Main.screenHeight / 2.0f, 0);
        }

        internal void ItemChanged(CustomItemSlot slot, ItemChangedEventArgs e) {
            Main.LocalPlayer.GetModPlayer<WingSlotPlayer>().ItemChanged(slot, e);
        }

        internal void ItemVisibilityChanged(CustomItemSlot slot, ItemVisibilityChangedEventArgs e) {
            Main.LocalPlayer.GetModPlayer<WingSlotPlayer>().ItemVisibilityChanged(slot, e);
        }

        internal void Unload() {
            EquipSlot.ItemChanged -= ItemChanged;
            SocialSlot.ItemChanged -= ItemChanged;
            DyeSlot.ItemChanged -= ItemChanged;
            EquipSlot.ItemVisibilityChanged -= ItemVisibilityChanged;
        }
    }
}
