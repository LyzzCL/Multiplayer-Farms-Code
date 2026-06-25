using HarmonyLib;
using MPF_Code.Config;
using MPF_Code.Farms;
using MPF_Code.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MPF_Code
{
  /// <summary>
  /// Main SMAPI entry point for initializing services, config validation, patches, and event hooks.
  /// </summary>
  internal sealed class ModEntry : Mod
  {
    public static ModEntry Instance { get; private set; } = null!;

    public ModConfig Config { get; set; } = null!;

    public override void Entry(IModHelper helper)
    {
      Instance = this;
      Config = helper.ReadConfig<ModConfig>();

      ModServices.Init(Monitor, helper);
      ConfigBinder.Validate();

      var harmony = new Harmony(ModManifest.UniqueID);
      PatchRegistrar.ApplyAll(harmony, Config);

      helper.Events.GameLoop.GameLaunched += OnGameLaunched;
      helper.Events.GameLoop.DayStarted += WorldSetup.OnDayStarted;
      helper.Events.Display.RenderedWorld += MailboxOverlay.OnRenderedWorld;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
      ConfigMenu.Register();

      var cp = Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");

      CpTokens.RegisterAll(cp!);
    }
  }
}
