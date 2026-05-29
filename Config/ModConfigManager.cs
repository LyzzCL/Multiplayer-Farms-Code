using System;
using ContentPatcher;
using GenericModConfigMenu;

namespace MPF_Code.Config
{
    internal partial class ModConfigManager
    {
        private readonly ModEntry _mod;
        private readonly FarmDefinition[] _farms;

        private const string GeneralPageId = "general";
        private const string FarmsPageId = "farms";
        private const string HarmonyPageId = "harmony";

        private const string GenericModConfigMenuId = "spacechase0.GenericModConfigMenu";
        private const string ContentPatcherId = "Pathoschild.ContentPatcher";

        public ModConfigManager(ModEntry mod)
        {
            _mod = mod;
            _farms = BuildFarmDefinitions();
        }

        public void RegisterConfigMenu()
        {
            var configMenu = _mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(GenericModConfigMenuId);
            if (configMenu is null)
                return;

            var customFarms = GetCustomFarms();

            configMenu.Register(
                mod: _mod.ModManifest,
                reset: () => _mod.Config = new ModConfig(),
                save: () => _mod.Helper.WriteConfig(_mod.Config));

            configMenu.AddPageLink(_mod.ModManifest, GeneralPageId, () => "General Settings",
                () => "Changes to the vanilla game to enhance multiplayer gameplay like starter cabins, buildings or items.");
            configMenu.AddPageLink(_mod.ModManifest, FarmsPageId, () => "Customize Farms",
                () => "Lets you customize or disable/enable the additional farms. Please keep in mind that disabling or enabling farms requires a save restart.");
            configMenu.AddPageLink(_mod.ModManifest, HarmonyPageId, () => "Other Patches",
                () => "Allows you to disable/enable harmony patches, if you are running into issues its probably because of these.");

            configMenu.AddPage(_mod.ModManifest, GeneralPageId, () => "General");
            AddGeneralOptions(configMenu);

            configMenu.AddPage(_mod.ModManifest, FarmsPageId, () => "Farms");
            configMenu.AddSectionTitle(_mod.ModManifest,
              text: () => "Farm Settings (Requires World/Game Restart)",
              tooltip: () =>
                "Where you can disable/enable or customize farm types as well as the warp exit coordinates.\n" +
                "These options only take effect before loading a save, and if you are adding or \n" +
                "changing a custom farm type it is recommended you restart the whole game.");
            
            var farms = _farms.ToList();

            for (int i = 0; i < farms.Count; i++)
            {
              var farm = farms[i];
              string pageId = $"Farm{i}";

              configMenu.AddPageLink(
                _mod.ModManifest,
                pageId,
                () => $"Farm {farm.DefaultNumber}",
                () => $"Default: {farm.DefaultName}"
              );
            }
            
            configMenu.AddPageLink( _mod.ModManifest, "",() => "Back");

            for (int i = 0; i < farms.Count; i++)
            {
              var farm = farms[i];
              string pageId = $"Farm{i}";

              configMenu.AddPage(
                _mod.ModManifest,
                pageId,
                () => farm.DefaultName
              );

              AddFarmSection(configMenu, farm, customFarms);
              configMenu.AddPageLink(_mod.ModManifest, FarmsPageId, () => "Back");
            }
            
            

            configMenu.AddPage(_mod.ModManifest, HarmonyPageId, () => "Harmony");
            AddHarmonyOptions(configMenu);
        }

        public void RegisterContentPatcherTokens()
        {
            var cpApi = _mod.Helper.ModRegistry.GetApi<IContentPatcherAPI>(ContentPatcherId);
            if (cpApi is null)
                return;

            cpApi.RegisterToken(_mod.ModManifest, "FreeCabins", getValue: () => new[] { BoolText(_mod.Config.Free_Cabins) });
            cpApi.RegisterToken(_mod.ModManifest, "StarterCabins", getValue: () => new[] { BoolText(_mod.Config.Starter_Cabins) });
            cpApi.RegisterToken(_mod.ModManifest, "StarterItems", getValue: () => new[] { BoolText(_mod.Config.Starter_Items) });
            cpApi.RegisterToken(_mod.ModManifest, "StarterBuilds", getValue: () => new[] { BoolText(_mod.Config.Starter_Builds) });
            cpApi.RegisterToken(_mod.ModManifest, "FixTrees", getValue: () => new[] { BoolText(_mod.Config.Fix_Trees) });
            cpApi.RegisterToken(_mod.ModManifest, "FixEvents", getValue: () => new[] { BoolText(_mod.Config.Fix_Events) });
            cpApi.RegisterToken(_mod.ModManifest, "FixFestivals", getValue: () => new[] { BoolText(_mod.Config.Fix_Festivals) });

            foreach (var farm in _farms)
                RegisterFarmTokens(cpApi, farm);
        }
    }
}
