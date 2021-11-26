using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;
using ThunderRoad;

namespace DungeonConfigurator
{
    [HarmonyPatch(typeof(WaveSpawner))]
    [HarmonyPatch("StartWave")]
    [HarmonyPatch(new Type[] { typeof(WaveData), typeof(float), typeof(bool) })]
    public static class WaveSpawnerPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            Logger.Detailed("Applying patch to WaveSpawner");
            FieldInfo waveDataField = typeof(WaveSpawner).GetField("waveData", BindingFlags.Instance | BindingFlags.Public);

            var codes = new List<CodeInstruction>(instructions);
            var codes_patched = new List<CodeInstruction>();
            Label checkEnd = ilg.DefineLabel();

            var checkPatchStart = codes[3];
            var checkPatchEnd = codes[6];
            var accessPatchStart = checkPatchEnd;
            bool accessReplace = false;

            for (int i=0; i < codes.Count(); i++)
            {
                CodeInstruction instruction = codes[i];
                bool use_original = true;

                if (instruction == checkPatchStart)
                {
                    Logger.Detailed("Patch start instruction for waveData found, applying patch");
                    codes_patched.Add(new CodeInstruction(
                        OpCodes.Ldarg_0));
                    codes_patched.Add(new CodeInstruction(
                        OpCodes.Ldfld, waveDataField));
                    codes_patched.Add(new CodeInstruction(
                        OpCodes.Brtrue_S, checkEnd));
                }
                else if (instruction == checkPatchEnd)
                {
                    Logger.Detailed("Label 'checkEnd' added");
                    instruction.labels.Add(checkEnd);
                }
                
                if(instruction == accessPatchStart)
                {
                    accessReplace = true;
                }

                if(accessReplace)
                {
                    if(instruction.opcode == OpCodes.Ldarg_1)
                    {
                        Logger.Detailed("Replacing Ldarg_1 usage with Ldarg_0");
                        codes_patched.Add(new CodeInstruction(
                            OpCodes.Ldarg_0));
                        codes_patched.Add(new CodeInstruction(
                            OpCodes.Ldfld, waveDataField));
                        use_original = false;
                    }
                }

                if(use_original)
                {
                    codes_patched.Add(instruction);
                }
            }

            return codes_patched.AsEnumerable();
        }
    }
}
