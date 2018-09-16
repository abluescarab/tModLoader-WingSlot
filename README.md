This mod adds a dedicated wing slot to the armor and accessories page of the inventory (next to the defense icon).

* Left-click a set of wings or a dye, then place it in the slot.
* Right-click to equip wings.
* Shift-right-click to equip wings in the vanity slot.
* Right-click vanity wings to swap with equipped wings.
* Left-click the eye icon on the equip slot to toggle visibility.

## Warning
Unequip your wings from the dedicated slot before updating or disabling this mod!

## Known Issues
* Yoraiz0r's Spell eye glow doesn't work

## Mod Compatibility
If your mod needs to override equipping functionality:
```csharp
Mod wingSlot = ModLoader.GetMod("WingSlot");

if(wingSlot != null) {
    wingSlot.Call(/* "add" or "remove" */, /* func<bool> returns true to equip/false to cancel */);
}
```

These functions are checked during `WingSlotPlayer.Slot_Conditions()`. If any of them returns false, `Slot_Conditions()` cancels equipping a pair of wings, so please ensure that your method is relatively bug-free.

## Credits
* jopojelly for code that draws a slot to the left of the dyes; for fixing multiplayer issues and various other bugs
* Boffin for TerraUI
* Zsashas for bug report (custom dye incompatibility)
* VVV101 for released updated version
* vizthex for suggesting slot location option