using System;
using BepInEx;
using HarmonyLib;

namespace TutorialLayerUnlock {
    [BepInPlugin("com.xuncikulv.casualties.tutoriallayerunlock", "Tutorial Layer Unlock", "1.0.0")]
    [BepInProcess("CasualtiesUnknown.exe")]
    public sealed class Plugin : BaseUnityPlugin {
        private Harmony harmony;
        private void Awake() {
            harmony = new Harmony("com.xuncikulv.casualties.tutoriallayerunlock");
            try {
                Type handler = AccessTools.TypeByName("TutorialHandler");
                if (handler == null) throw new InvalidOperationException("TutorialHandler not found.");
                var start = AccessTools.Method(handler, "StartSelectedCourse", Type.EmptyTypes);
                var menu = AccessTools.Method(handler, "HandleCourseSelect", Type.EmptyTypes);
                if (start == null || menu == null) throw new InvalidOperationException("Expected tutorial methods not found.");
                harmony.Patch(start, transpiler: new HarmonyMethod(typeof(TutorialUnlockPatch), "StartCourse"));
                harmony.Patch(menu, transpiler: new HarmonyMethod(typeof(TutorialUnlockPatch), "CourseMenu"));
                Logger.LogInfo("Tutorial layer requirements removed. Save progression is unchanged.");
            } catch (Exception ex) {
                harmony.UnpatchSelf();
                Logger.LogError("Tutorial unlock disabled: " + ex);
            }
        }
        private void OnDestroy() { if (harmony != null) harmony.UnpatchSelf(); }
    }
}
