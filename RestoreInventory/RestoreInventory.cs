using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Plugins;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using PlayerInventory = RestoreInventory.Models.PlayerInventory;

// For more, visit https://openmod.github.io/openmod-docs/devdoc/guides/getting-started.html

[assembly: PluginMetadata("F.RestoreInventory", DisplayName = "RestoreInventory")]
namespace RestoreInventory
{
    public class RestoreInventory : OpenModUnturnedPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly ILogger<RestoreInventory> m_Logger;

        public Dictionary<Player, SteamPlayer> Restorer = new Dictionary<Player, SteamPlayer>();
        private List<PlayerInventory> Data = new List<PlayerInventory>();

        public RestoreInventory(
            IConfiguration configuration,
            ILogger<RestoreInventory> logger,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_Configuration = configuration;
            m_Logger = logger;
        }

        protected override async UniTask OnLoadAsync()
        {
            if (!await DataStore.ExistsAsync("Inventories"))
            {
                await DataStore.SaveAsync("Inventories", new List<PlayerInventory>());
            }
            else
            {
                var data = await DataStore.LoadAsync<List<PlayerInventory>>("Inventories");
                if (data != null)
                {
                    Data = data;
                }
            }

            Restorer = new Dictionary<Player, SteamPlayer>();
            EffectManager.onEffectButtonClicked += OnClicked;
            m_Logger.LogInformation("Restore Inventory Plugin Loaded Correctly");
        }

        private void OnClicked(Player player, string buttonName)
        {
            UniTask.SwitchToMainThread();
            switch (buttonName)
            {
                case "restore1":
                    var selected = Restorer[player];
                    var inventories = GetInventories().Where(x => x.PlayerId == player.channel.owner.playerID.steamID.ToString()).OrderBy(x => x.Date).ToList();
                    var inventory = new Models.PlayerInventory();

                    inventory = inventories[0];

                    if(inventory.Clothing == null)
                    {
                        return;
                    }
                    if(inventory.Items == null) { return; }

                    foreach (var cloth in inventory.Clothing)
                    {
                        selected.player.inventory.tryAddItem(new Item(cloth, true), true);
                    }

                    foreach (var item in inventory.Items)
                    {
                        selected.player.inventory.tryAddItem(
                            new Item(item.Id, item.Amount, item.Quality, Convert.FromBase64String(item.State)), item.X, item.Y,
                            item.Page, item.Rot);
                    }
                    EffectManager.askEffectClearByID(3331, player.channel.owner.playerID.steamID);
                    player.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);

                    break;
                case "restore2":

                    var selected2 = Restorer[player];
                    var inventories2 = GetInventories().Where(x => x.PlayerId == player.channel.owner.playerID.steamID.ToString()).OrderBy(x => x.Date).ToList();
                    var inventory2 = new Models.PlayerInventory();

                    inventory2 = inventories2[1];

                    if (inventory2.Clothing == null)
                    {
                        return;
                    }
                    if (inventory2.Items == null) { return; }

                    foreach (var cloth in inventory2.Clothing)
                    {
                        selected2.player.inventory.tryAddItem(new Item(cloth, true), true);
                    }

                    foreach (var item in inventory2.Items)
                    {
                        selected2.player.inventory.tryAddItem(
                            new Item(item.Id, item.Amount, item.Quality, Convert.FromBase64String(item.State)), item.X, item.Y,
                            item.Page, item.Rot);
                    }
                    EffectManager.askEffectClearByID(3331, player.channel.owner.playerID.steamID);
                    player.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);


                    break;
                case "restore3":
                    var selected3 = Restorer[player];
                    var inventories3 = GetInventories().Where(x => x.PlayerId == player.channel.owner.playerID.steamID.ToString()).OrderBy(x => x.Date).ToList();
                    var inventory3 = new Models.PlayerInventory();

                    inventory3 = inventories3[2];

                    if (inventory3.Clothing == null)
                    {
                        return;
                    }
                    if (inventory3.Items == null) { return; }

                    foreach (var cloth in inventory3.Clothing)
                    {
                        selected3.player.inventory.tryAddItem(new Item(cloth, true), true);
                    }

                    foreach (var item in inventory3.Items)
                    {
                        selected3.player.inventory.tryAddItem(
                            new Item(item.Id, item.Amount, item.Quality, Convert.FromBase64String(item.State)), item.X, item.Y,
                            item.Page, item.Rot);
                    }
                    EffectManager.askEffectClearByID(3331, player.channel.owner.playerID.steamID);
                    player.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);

                    break;
                case "close":
                    EffectManager.askEffectClearByID(3331, player.channel.owner.playerID.steamID);
                    player.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);
                    break;
            }
        }

        public List<PlayerInventory> GetInventories()
        {
            return Data;
        }

        public Task AddInventory(PlayerInventory inventory)
        {
            Data.Add(inventory);
            DataStore.SaveAsync("Inventories", Data);
            return Task.CompletedTask;
        }

        public void RemoveInventory(PlayerInventory inventory)
        {
            Data.Remove(inventory);
            DataStore.SaveAsync("Inventories", Data);
        }

        protected override UniTask OnUnloadAsync()
        {
            EffectManager.onEffectButtonClicked -= OnClicked;
            m_Logger.LogInformation("Restore Inventory Plugin Unloaded Correctly");
            return UniTask.CompletedTask;
        }
    }
}
