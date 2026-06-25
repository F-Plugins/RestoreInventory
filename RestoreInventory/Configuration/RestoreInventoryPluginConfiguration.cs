using Rocket.API;

namespace Feli.RestoreInventory.Configuration;

public class RestoreInventoryPluginConfiguration : IRocketPluginConfiguration
{
    public int MaxInventoriesToKeep { get; set; }

    public void LoadDefaults()
    {
        MaxInventoriesToKeep = 3;
    }

    public int GetMaxInventoriesToKeep()
    {
        if (MaxInventoriesToKeep == -1)
            return -1;

        if (MaxInventoriesToKeep < 1)
            return 1;

        return MaxInventoriesToKeep;
    }
}
