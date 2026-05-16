using BepInEx;
using BepInEx.Unity.IL2CPP;
using LilithsHeart;
using LilithsCookbook.Data;
using LilithsCookbook.Systems;

namespace LilithsCookbook;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("audaciousbovine.lilithsheart")]
public class Plugin : BasePlugin
{
    public static RecipeConfig? RecipeConfig { get; private set; }

    public override void Load()
    {
        LilithsLogger.Info($"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loaded.");
        RecipeConfig = ConfigLoader.Load();
    }

    public override bool Unload()
    {
        return true;
    }
}