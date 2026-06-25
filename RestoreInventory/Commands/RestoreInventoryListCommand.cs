using Feli.RestoreInventory.Extensions;
using Feli.Rocket.Utils.Threading;
using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Feli.RestoreInventory.Commands;

public class RestoreInventoryListCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length == 0)
        {
            caller.SendLocalizedMessage("RestoreInventoryListSyntax", Color.red);
            return;
        }

        var player = UnturnedPlayer.FromName(command[0]);

        if (player is null)
        {
            caller.SendLocalizedMessage("RestoreInventoryPlayerNotFound", Color.red, command[0]);
            return;
        }

        ThreadTool.RunOnThreadPool(async () =>
        {
            var inventories = (await RestoreInventoryPlugin.Instance.DatabaseProvider.GetPlayerInventoriesAsync(player.CSteamID.m_SteamID)).ToArray();

            ThreadTool.QueueOnMainThread(() =>
            {
                if (inventories.Length == 0)
                {
                    caller.SendLocalizedMessage("RestoreInventoryListEmpty", Color.red, player.DisplayName);
                    return;
                }

                caller.SendLocalizedMessage("RestoreInventoryListHeader", Color.cyan, player.DisplayName);

                for (var i = 0; i < inventories.Length; i++)
                {
                    var inventory = inventories[i];
                    caller.SendLocalizedMessage("RestoreInventoryListItem", Color.cyan, i + 1, inventory.ItemCount, inventory.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            });
        });
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;

    public string Name => "restoreinventorylist";

    public string Help => "List a player's saved inventories";

    public string Syntax => "<player>";

    public List<string> Aliases => new() { "ril", "rinventories" };

    public List<string> Permissions => new();
}
