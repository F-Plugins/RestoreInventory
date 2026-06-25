# RestoreInventory
![Discord](https://img.shields.io/discord/742861338233274418?label=Discord&logo=Discord) [![Github All Releases](https://img.shields.io/github/downloads/F-Plugins/RestoreInventory/total?label=Github%20Downloads)]()

RestoreInventory is a RocketMod plugin for Unturned that saves player inventories when they die and lets players restore saved inventories later.

Inventories are stored locally with LiteDB in the plugin directory.

## Features

- Saves player inventory snapshots on death
- Stores inventory data in LiteDB
- Keeps the newest saved inventories per player
- Supports unlimited saved inventories with config value `-1`
- Lists saved inventories newest to oldest
- Restores by the displayed inventory number
- Merges restored items into the current inventory
- Drops restored items at the player position if they do not fit

## Commands

### `/restoreinventorylist <player>`

Lists a player's saved inventories from newest to oldest.

Aliases:

- `/ril`
- `/rinventories`

Each entry shows:

- inventory number
- item count
- saved date/time

### `/restoreinventory <player> <number>`

Restores the saved inventory matching the number shown by `/restoreinventorylist <player>` to the target player.

Alias:

- `/ri`

Example:

```text
/restoreinventory Feli 1
```

## Permissions

No permissions are required by default.

## Configuration

Default configuration:

```xml
<RestoreInventoryPluginConfiguration>
  <MaxInventoriesToKeep>3</MaxInventoriesToKeep>
</RestoreInventoryPluginConfiguration>
```

### `MaxInventoriesToKeep`

Controls how many saved inventories are kept per player.

- Default: `3`
- Minimum: `1`
- Unlimited: `-1`

If a value lower than `1` is configured, except `-1`, the plugin treats it as `1`.

## Storage

The plugin stores data in:

```text
restoreinventory.db
```

Each saved inventory includes:

- Steam64 player ID
- saved date/time
- inventory items with page, position, rotation, amount, quality, and state
- equipped clothing with clothing type, item ID, quality, and state

Empty clothing slots are not stored.

## Restore Behavior

Restoring does not clear the player's current inventory.

For saved inventory items, the plugin tries to:

1. Place the item back in its original page and position.
2. Add the item anywhere it fits in the player's inventory.
3. Drop the item at the player's position if it does not fit.

For saved clothing, the plugin tries to:

1. Equip it if the matching clothing slot is empty.
2. Add it anywhere it fits in the player's inventory.
3. Drop it at the player's position if it does not fit.

Restoring an inventory does not delete it from the database.

## Building

Build locally with:

```powershell
dotnet build RestoreInventory.sln --configuration Release
```
