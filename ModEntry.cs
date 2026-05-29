using HarmonyLib;
using MPF_Code.Config;
using MPF_Code.Patches;
using StardewModdingAPI;

namespace MPF_Code
{
  internal sealed partial class ModEntry : Mod
  {
    public static ModEntry Instance { get; private set; } = null!;
    public ModConfig Config { get; set; } = null!;

    public override void Entry(IModHelper helper)
    {
      Instance = this;
      Config = helper.ReadConfig<ModConfig>();
      
      MailboxPositionPatch.Helper = helper;

      var harmony = new Harmony(ModManifest.UniqueID);
      RegisterPatches.ApplyAll(harmony, Config);

      helper.Events.GameLoop.GameLaunched += OnGameLaunched;
      helper.Events.Display.RenderedWorld += OnRenderedWorld;
      helper.Events.GameLoop.DayStarted += OnDayStarted;
      
    }
  }
}
