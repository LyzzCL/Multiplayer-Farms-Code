namespace MPF_Code.Config
{
  internal partial class ModConfigManager
  {
    private FarmDefinition[] BuildFarmDefinitions()
{
  return new[]
  {
    new FarmDefinition(
      SectionTitle: "Farm 1 - Wilderness (Next to Bookseller)",
      DefaultNumber: "1",
      DefaultName: "Wilderness",
      EnabledTokenName: "FarmOne",
      NameTokenName: "FOneName",
      TypeTokenName: "FOneType",
      GetEnabled: () => _mod.Config.F1_Enabled,
      SetEnabled: value => _mod.Config.F1_Enabled = value,
      GetName: () => _mod.Config.F1_Name,
      SetName: value => _mod.Config.F1_Name = value,
      GetFarmType: () => _mod.Config.F1_Type,
      SetFarmType: value => _mod.Config.F1_Type = value,
      Warps: new[]
      {
        new WarpSlot(
          Label: "Exit Coords",
          Tooltip: "Default: (0, 12) - To Bookseller",
          TokenPrefix: "FOne",
          GetCoords: () => _mod.Config.F1_Coords_1,
          SetCoords: value => _mod.Config.F1_Coords_1 = value,
          GetWarpMode: () => _mod.Config.F1_Coords_1_Mode,
          SetWarpMode: value => _mod.Config.F1_Coords_1_Mode = value),
      }),

    new FarmDefinition(
      SectionTitle: "Farm 2 - Forest (Near Mage Tower & Secret Woods)",
      DefaultNumber: "2",
      DefaultName: "Forest",
      EnabledTokenName: "FarmTwo",
      NameTokenName: "FTwoName",
      TypeTokenName: "FTwoType",
      GetEnabled: () => _mod.Config.F2_Enabled,
      SetEnabled: value => _mod.Config.F2_Enabled = value,
      GetName: () => _mod.Config.F2_Name,
      SetName: value => _mod.Config.F2_Name = value,
      GetFarmType: () => _mod.Config.F2_Type,
      SetFarmType: value => _mod.Config.F2_Type = value,
      Warps: new[]
      {
        new WarpSlot(
          Label: "Exit Coords 1",
          Tooltip: "Default: (79, 46) - To Mage Tower",
          TokenPrefix: "FTwo",
          GetCoords: () => _mod.Config.F2_Coords_1,
          SetCoords: value => _mod.Config.F2_Coords_1 = value,
          GetWarpMode: () => _mod.Config.F2_Coords_1_Mode,
          SetWarpMode: value => _mod.Config.F2_Coords_1_Mode = value),

        new WarpSlot(
          Label: "Exit Coords 2",
          Tooltip: "Default: (79, 15) - To Secret Woods",
          TokenPrefix: "FTwoTwo",
          GetCoords: () => _mod.Config.F2_Coords_2,
          SetCoords: value => _mod.Config.F2_Coords_2 = value,
          GetWarpMode: () => _mod.Config.F2_Coords_2_Mode,
          SetWarpMode: value => _mod.Config.F2_Coords_2_Mode = value),
      }),

    new FarmDefinition(
      SectionTitle: "Farm 3 - Beach (Below Museum & Beach East Side)",
      DefaultNumber: "3",
      DefaultName: "Beach",
      EnabledTokenName: "FarmThree",
      NameTokenName: "FTreName",
      TypeTokenName: "FTreType",
      GetEnabled: () => _mod.Config.F3_Enabled,
      SetEnabled: value => _mod.Config.F3_Enabled = value,
      GetName: () => _mod.Config.F3_Name,
      SetName: value => _mod.Config.F3_Name = value,
      GetFarmType: () => _mod.Config.F3_Type,
      SetFarmType: value => _mod.Config.F3_Type = value,
      Warps: new[]
      {
        new WarpSlot(
          Label: "Exit Coords 1",
          Tooltip: "Default: (7, 0)  - To Town",
          TokenPrefix: "FTre",
          GetCoords: () => _mod.Config.F3_Coords_1,
          SetCoords: value => _mod.Config.F3_Coords_1 = value,
          GetWarpMode: () => _mod.Config.F3_Coords_1_Mode,
          SetWarpMode: value => _mod.Config.F3_Coords_1_Mode = value),

        new WarpSlot(
          Label: "Exit Coords 2",
          Tooltip: "Default: (0, 19) - To Beach",
          TokenPrefix: "FTreTwo",
          GetCoords: () => _mod.Config.F3_Coords_2,
          SetCoords: value => _mod.Config.F3_Coords_2 = value,
          GetWarpMode: () => _mod.Config.F3_Coords_2_Mode,
          SetWarpMode: value => _mod.Config.F3_Coords_2_Mode = value),
      }),

    new FarmDefinition(
      SectionTitle: "Farm 4 - Riverland (Haley House & Beach West Side)",
      DefaultNumber: "4",
      DefaultName: "Riverland",
      EnabledTokenName: "FarmFour",
      NameTokenName: "FourName",
      TypeTokenName: "FourType",
      GetEnabled: () => _mod.Config.F4_Enabled,
      SetEnabled: value => _mod.Config.F4_Enabled = value,
      GetName: () => _mod.Config.F4_Name,
      SetName: value => _mod.Config.F4_Name = value,
      GetFarmType: () => _mod.Config.F4_Type,
      SetFarmType: value => _mod.Config.F4_Type = value,
      Warps: new[]
      {
        new WarpSlot(
          Label: "Exit Coords 1",
          Tooltip: "Default: (40, 0) - To Haley's house",
          TokenPrefix: "Four",
          GetCoords: () => _mod.Config.F4_Coords_1,
          SetCoords: value => _mod.Config.F4_Coords_1 = value,
          GetWarpMode: () => _mod.Config.F4_Coords_1_Mode,
          SetWarpMode: value => _mod.Config.F4_Coords_1_Mode = value),

        new WarpSlot(
          Label: "Exit Coords 2",
          Tooltip: "Default: (40, 64) - To Beach",
          TokenPrefix: "FourTwo",
          GetCoords: () => _mod.Config.F4_Coords_2,
          SetCoords: value => _mod.Config.F4_Coords_2 = value,
          GetWarpMode: () => _mod.Config.F4_Coords_2_Mode,
          SetWarpMode: value => _mod.Config.F4_Coords_2_Mode = value),
      }),

    new FarmDefinition(
      SectionTitle: "Farm 5 - Meadowlands (On Backwoods West)",
      DefaultNumber: "5",
      DefaultName: "Meadowlands",
      EnabledTokenName: "FarmFive",
      NameTokenName: "FiveName",
      TypeTokenName: "FiveType",
      GetEnabled: () => _mod.Config.F5_Enabled,
      SetEnabled: value => _mod.Config.F5_Enabled = value,
      GetName: () => _mod.Config.F5_Name,
      SetName: value => _mod.Config.F5_Name = value,
      GetFarmType: () => _mod.Config.F5_Type,
      SetFarmType: value => _mod.Config.F5_Type = value,
      Warps: new[]
      {
        new WarpSlot(
          Label: "Exit Coords",
          Tooltip: "Default: (99, 21) - To Backwoods",
          TokenPrefix: "Five",
          GetCoords: () => _mod.Config.F5_Coords_1,
          SetCoords: value => _mod.Config.F5_Coords_1 = value,
          GetWarpMode: () => _mod.Config.F5_Coords_1_Mode,
          SetWarpMode: value => _mod.Config.F5_Coords_1_Mode = value),
      }),

    new FarmDefinition(
      SectionTitle: "Farm 6 - Hilltop (Next to Linus & Railroad",
      DefaultNumber: "6",
      DefaultName: "Hilltop",
      EnabledTokenName: "FarmSix",
      NameTokenName: "FSixName",
      TypeTokenName: "FSixType",
      GetEnabled: () => _mod.Config.F6_Enabled,
      SetEnabled: value => _mod.Config.F6_Enabled = value,
      GetName: () => _mod.Config.F6_Name,
      SetName: value => _mod.Config.F6_Name = value,
      GetFarmType: () => _mod.Config.F6_Type,
      SetFarmType: value => _mod.Config.F6_Type = value,
      Warps: new[]
      {
        new WarpSlot(
          Label: "Exit Coords 1",
          Tooltip: "Default: (40, 64) - To Mountains",
          TokenPrefix: "FSix",
          GetCoords: () => _mod.Config.F6_Coords_1,
          SetCoords: value => _mod.Config.F6_Coords_1 = value,
          GetWarpMode: () => _mod.Config.F6_Coords_1_Mode,
          SetWarpMode: value => _mod.Config.F6_Coords_1_Mode = value),

        new WarpSlot(
          Label: "Exit Coords 2",
          Tooltip: "Default: (0, 10) - To Railroad",
          TokenPrefix: "FSixTwo",
          GetCoords: () => _mod.Config.F6_Coords_2,
          SetCoords: value => _mod.Config.F6_Coords_2 = value,
          GetWarpMode: () => _mod.Config.F6_Coords_2_Mode,
          SetWarpMode: value => _mod.Config.F6_Coords_2_Mode = value),
      }),

    new FarmDefinition(
      SectionTitle: "Farm 7 - 4 Corners (On Backwoods & Railroad)",
      DefaultNumber: "7",
      DefaultName: "Four Corners",
      EnabledTokenName: "FarmSeven",
      NameTokenName: "FSvnName",
      TypeTokenName: "FSvnType",
      GetEnabled: () => _mod.Config.F7_Enabled,
      SetEnabled: value => _mod.Config.F7_Enabled = value,
      GetName: () => _mod.Config.F7_Name,
      SetName: value => _mod.Config.F7_Name = value,
      GetFarmType: () => _mod.Config.F7_Type,
      SetFarmType: value => _mod.Config.F7_Type = value,
      Warps: new[]
      {
        new WarpSlot(
          Label: "Exit Coords 1",
          Tooltip: "Default: (40, 64) - To Backwoods",
          TokenPrefix: "FSvn",
          GetCoords: () => _mod.Config.F7_Coords_1,
          SetCoords: value => _mod.Config.F7_Coords_1 = value,
          GetWarpMode: () => _mod.Config.F7_Coords_1_Mode,
          SetWarpMode: value => _mod.Config.F7_Coords_1_Mode = value),

        new WarpSlot(
          Label: "Exit Coords 2",
          Tooltip: "Default: (79, 17) - To Railroad",
          TokenPrefix: "FSvnTwo",
          GetCoords: () => _mod.Config.F7_Coords_2,
          SetCoords: value => _mod.Config.F7_Coords_2 = value,
          GetWarpMode: () => _mod.Config.F7_Coords_2_Mode,
          SetWarpMode: value => _mod.Config.F7_Coords_2_Mode = value),
      }),
  };
}

    private sealed record FarmDefinition(
      string SectionTitle,
      string DefaultNumber,
      string DefaultName,
      string EnabledTokenName,
      string NameTokenName,
      string TypeTokenName,
      System.Func<bool> GetEnabled,
      System.Action<bool> SetEnabled,
      System.Func<string> GetName,
      System.Action<string> SetName,
      System.Func<string> GetFarmType,
      System.Action<string> SetFarmType,
      WarpSlot[] Warps);

    private sealed record WarpSlot(
      string Label,
      string Tooltip,
      string TokenPrefix,
      System.Func<string> GetCoords,
      System.Action<string> SetCoords,
      System.Func<string> GetWarpMode,
      System.Action<string> SetWarpMode);
  }
}
