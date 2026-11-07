using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Mono.Cecil;
using TutorialLayerUnlock;

namespace UnityEngine {
    public static class PlayerPrefs {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int GetInt(string key) { return 0; }
    }
}
public static class TutorialFixture {
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool StartSelectedCourse() { return UnityEngine.PlayerPrefs.GetInt("deepestlayer") >= 1; }
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int HandleCourseSelect() {
        int depth = UnityEngine.PlayerPrefs.GetInt("deepestlayer");
        return (depth >= 2 ? 1 : 0) + (UnityEngine.PlayerPrefs.GetInt("deepestlayer") >= 2 ? 1 : 0) + UnityEngine.PlayerPrefs.GetInt("tutorial_complete");
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int UnrelatedDepthCheck() { return UnityEngine.PlayerPrefs.GetInt("deepestlayer"); }
}
class Verify {
    static int checks;
    static void Check(bool value,string message){if(!value)throw new Exception("FAIL: "+message);Console.WriteLine("PASS: "+message);checks++;}
    static bool IsDepthRead(Mono.Cecil.Cil.Instruction instruction){
        var method=instruction.Operand as MethodReference;
        return instruction.OpCode==Mono.Cecil.Cil.OpCodes.Call && method!=null && method.FullName=="System.Int32 UnityEngine.PlayerPrefs::GetInt(System.String)" && instruction.Previous!=null && (string)instruction.Previous.Operand=="deepestlayer";
    }
    static int Main(string[] args){
        try{
            string before=EngineHash(args[0]);
            using(var assembly=AssemblyDefinition.ReadAssembly(args[0])){
                var type=assembly.MainModule.Types.Single(t=>t.FullName=="TutorialHandler");
                Check(type.Methods.Single(m=>m.Name=="StartSelectedCourse").Body.Instructions.Count(IsDepthRead)==1,"actual game has one start-course depth check");
                Check(type.Methods.Single(m=>m.Name=="HandleCourseSelect").Body.Instructions.Count(IsDepthRead)==2,"actual game has two menu depth checks");
            }
            Check(!TutorialFixture.StartSelectedCourse()&&TutorialFixture.HandleCourseSelect()==0,"locked before patch at depth zero");
            var harmony=new Harmony("tutorial-unlock-verification");
            harmony.Patch(typeof(TutorialFixture).GetMethod("StartSelectedCourse"),transpiler:new HarmonyMethod(typeof(TutorialUnlockPatch),"StartCourse"));
            harmony.Patch(typeof(TutorialFixture).GetMethod("HandleCourseSelect"),transpiler:new HarmonyMethod(typeof(TutorialUnlockPatch),"CourseMenu"));
            Check(TutorialFixture.StartSelectedCourse(),"course starts at depth zero after real Harmony patch");
            Check(TutorialFixture.HandleCourseSelect()==2,"both menu locks removed; other key remains zero");
            Check(TutorialFixture.UnrelatedDepthCheck()==0&&UnityEngine.PlayerPrefs.GetInt("deepestlayer")==0,"global saved depth reads remain unchanged");
            harmony.UnpatchSelf();
            Check(!TutorialFixture.StartSelectedCourse()&&TutorialFixture.HandleCourseSelect()==0,"removing patch restores original locks");
            bool refused=false;try{TutorialUnlockPatch.Rewrite(new[]{new CodeInstruction(OpCodes.Ret)},1).ToList();}catch(InvalidOperationException){refused=true;}
            Check(refused,"refuse unexpected method layout");
            Check(before==EngineHash(args[0]),"game assembly unchanged");
            Console.WriteLine("ALL "+checks+" CHECKS PASSED");return 0;
        }catch(Exception ex){Console.WriteLine(ex);return 1;}
    }
    static string EngineHash(string file){using(var h=System.Security.Cryptography.SHA256.Create())using(var s=File.OpenRead(file))return BitConverter.ToString(h.ComputeHash(s));}
}
