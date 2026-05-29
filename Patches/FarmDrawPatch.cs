using HarmonyLib;
using System.Reflection.Emit;
using StardewValley;

namespace MPF_Code.Patches
{
  internal static class FarmDrawPatch
  {
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
      CodeMatcher matcher = new(instructions);

      matcher.MatchStartForward(
          new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.mailbox))),
          new CodeMatch(OpCodes.Callvirt, typeof(ICollection<string>).GetMethod("get_Count")))
        .Insert(
          new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.IsMasterGame))),
          new CodeInstruction(OpCodes.Brfalse, matcher.InstructionAt(matcher.Pos + 5).labels.FirstOrDefault()));

      return matcher.InstructionEnumeration();
    }
  }
}
