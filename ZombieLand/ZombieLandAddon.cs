namespace ZombieLand_Addon
{
    using Verse;
    using HarmonyLib;
    using System.Reflection;

    public class ZombieLandAddon : Mod
    {
        public ZombieLandAddon(ModContentPack content) : base(content)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411.{assembly.GetName().Name}.{assembly.GetName().Version}").PatchAll(assembly);
        }
    }
}
