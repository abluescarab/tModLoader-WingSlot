This mod adds dedicated Utility Slots (wings, boots, balloon) to the inventory. 
These 3 types of accessories often feel like quality-of-life improvements that it really sucks to lose as you get cool build-defining and boss accessories.

Majority of the work can be credited to [abluescarab](https://github.com/abluescarab/tModLoader-WingSlot), as this was just building on top.

Notes:
June 21 2022:
* Uploading first release version
* Not currently tested in multiplayer, but seems like it should work
* Some config options removed for simplicity of development, may reenable eventually

The rest of this readme is copied unchanged from their repo:

Now includes a config file! Access it from the in-game Mod Configuration.

* Left-click a set of wings or a dye, then place it in the slot.
* Right-click to equip wings.
* Shift-right-click to equip wings in the vanity slot.
* Right-click vanity wings to swap with equipped wings.
* Left-click the eye icon on the equip slot to toggle visibility.

## Warning
Unequip your wings from the dedicated slot before updating or disabling this mod!

## Known Issues
* Yoraiz0r's Spell eye glow doesn't work
* Wings and character movement while flying will not display correctly in multiplayer
* Custom location will sometimes move off screen when resizing window (use mod config to reset if needed)

## Mod Compatibility
If your mod needs to access the items in the slots:
```csharp
Item wingItem = null;

if (ModLoader.GetMod("WingSlot") is Mod wingSlot && wingSlot != null) {
    wingItem = (Item)wingSlot.Call(/* "getVisible"/"getVanity"/"getEquip" */, player.whoAmI);
}
```

If your mod needs to override right-click functionality:
```csharp
Mod wingSlot = ModLoader.GetMod("WingSlot");

if(wingSlot != null) {
    wingSlot.Call(/* "add" or "remove" */, /* func<bool> returns true to cancel/false to continue */);
}
```

These functions are checked during `GlobalWingItem.CanRightClick()`. If any of them returns true, `CanRightClick()` cancels equipping a pair of wings, so please ensure that your method is relatively bug-free.

## Credits
* Blockaroz for the fantastic mod icon!
* jopojelly for code that draws a slot to the left of the dyes; for fixing various other bugs
* Boffin for TerraUI
* Zsashas for bug report (custom dye incompatibility)
* VVV101 for released updated version
* vizthex for suggesting slot location option
* jofairden for suggesting mod compatibility right-click overrides
* direwolf420 for fixing the mod for 0.11.5 and adding getequipslotitem & other mod.Call()s
* Lastprismer for the Chinese translation