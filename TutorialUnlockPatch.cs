using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace TutorialLayerUnlock {
    public static class TutorialUnlockPatch {
        // Replaces only the depth read inside the two tutorial methods.
        // No PlayerPrefs writes, course completion, or global getter patches.
        public static int UnrestrictedTutorialDepth(string unusedKey) { return Int32.MaxValue; }

        public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> instructions, int expectedReads) {
            var code = instructions.Select(x => new CodeInstruction(x)).ToList();
            int replaced = 0;
            for (int i = 1; i < code.Count; i++) {
                var call = code[i].operand as MethodInfo;
                if (code[i].opcode != OpCodes.Call || call == null || call.DeclaringType == null ||
                    call.DeclaringType.FullName != "UnityEngine.PlayerPrefs" || call.Name != "GetInt" ||
                    call.ReturnType != typeof(int) || !call.IsStatic) continue;
                var parameters = call.GetParameters();
                if (parameters.Length != 1 || parameters[0].ParameterType != typeof(string)) continue;
                int previous = i - 1;
                while (previous >= 0 && code[previous].opcode == OpCodes.Nop) previous--;
                if (previous < 0 || code[previous].opcode != OpCodes.Ldstr || (string)code[previous].operand != "deepestlayer") continue;
                code[i].operand = typeof(TutorialUnlockPatch).GetMethod("UnrestrictedTutorialDepth");
                replaced++;
            }
            if (replaced != expectedReads)
                throw new InvalidOperationException("Tutorial depth checks changed: expected " + expectedReads + ", found " + replaced + ". Unsupported game/mod version; patch refused.");
            return code;
        }
        public static IEnumerable<CodeInstruction> StartCourse(IEnumerable<CodeInstruction> instructions) { return Rewrite(instructions, 1); }
        public static IEnumerable<CodeInstruction> CourseMenu(IEnumerable<CodeInstruction> instructions) { return Rewrite(instructions, 2); }
    }
}
