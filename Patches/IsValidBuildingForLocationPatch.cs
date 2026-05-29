namespace MPF_Code.Patches
{
  internal static class IsValidBuildingForLocationPatch
  {
    public static void Postfix(string typeId, ref bool __result)
    {
      if (typeId == "Cabin")
        __result = true;
    }
  }
}
