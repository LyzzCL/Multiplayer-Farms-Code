using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace MPF_Code.Patches
{
  internal static class FarmDrawPatch
  {
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
      ILGenerator generator)
    {
      List<CodeInstruction> original = instructions.ToList();

      try
      {
        CodeMatcher matcher = new(original);

        matcher.MatchStartForward(
          new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.mailbox))),
          new CodeMatch(OpCodes.Callvirt, typeof(ICollection<string>).GetMethod("get_Count")));

        if (matcher.IsInvalid)
        {
          ModServices.Monitor.LogOnce(
            "FarmDraw transpiler: mailbox-count pattern not found; leaving Farm.draw unmodified.", LogLevel.Warn);

          return original;
        }

        Label? skipMailboxDrawLabel = null;

        for (int i = matcher.Pos + 2; i < Math.Min(original.Count, matcher.Pos + 12); i++)
        {
          if (original[i].opcode.FlowControl == FlowControl.Cond_Branch && original[i].operand is Label label)
          {
            skipMailboxDrawLabel = label;
            break;
          }
        }

        if (skipMailboxDrawLabel is null)
        {
          ModServices.Monitor.LogOnce(
            "FarmDraw transpiler: mailbox-count branch not found; leaving Farm.draw unmodified.", LogLevel.Warn);

          return original;
        }

        matcher.Insert(
          new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.IsMasterGame))),
          new CodeInstruction(OpCodes.Brfalse, skipMailboxDrawLabel.Value));

        return matcher.InstructionEnumeration();
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"FarmDraw transpiler failed ({ex.Message}); leaving Farm.draw unmodified.",
          LogLevel.Error);

        return original;
      }
    }
  }
}
