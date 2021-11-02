namespace ZombieLand_Addon.Patches
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using HarmonyLib;
    using ToolkitCreator;
    using ZombieLand_Addon.ActionWorkers;

    [HarmonyPatch(typeof(Event), nameof(Event.FireEvents))]
    public static class Event_FireEvents_Patches
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codepoint = -1;

            var codeInstructions = instructions.ToList();
            var codes = new List<CodeInstruction>(codeInstructions);
            for(var i = 0; i < codeInstructions.Count()-2; i++)
            {
                var currentCode = codes[i];
                var nextCode = codes[i+1];
                var afterCode = codes[i+2];
                if(codepoint == -1)
                {
                    if (currentCode.opcode != OpCodes.Ldfld) 
                        continue;

                    if(nextCode.opcode != OpCodes.Call || afterCode.opcode != OpCodes.Castclass)
                        continue;

                    System.Console.WriteLine("Found Position attempting to change!");
                    codepoint = i;
                    codes[i] = new CodeInstruction(OpCodes.Callvirt, typeof(Event_FireEvents_Patches).GetMethod("CreateInstanceReplace"));
                }
                else if(i == (codepoint + 1))
                {
                    codes[i] = new CodeInstruction(OpCodes.Nop);
                }
            }

            if(codepoint > -1)
                System.Console.WriteLine($"Event_FireEvents Transpiler Found and Patched.");
            else
                throw new System.Exception("ZombieLand_Addon's ToolKitCreator Transpiler injection point NOT found!!  ToolKitCreator has most likely updated and has broken this mod!");

            return codes.AsEnumerable();
        }

        public static object CreateInstanceReplace(Action named)
        {
            if(named is ZombieAction zombieAction)
            {
                ZombieAttack zombieAttack = (ZombieAttack)System.Activator.CreateInstance(named.actionWorker);
                zombieAttack.ZombieAction = zombieAction;
                return zombieAttack;
            }

            return System.Activator.CreateInstance(named.actionWorker);
        }
    }
}
