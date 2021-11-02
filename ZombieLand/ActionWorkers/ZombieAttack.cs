namespace ZombieLand_Addon.ActionWorkers
{
    using Verse;
    using UnityEngine;
    using System.Collections;
    using ZombieLand;
    using ToolkitCreator.ActionWorkers;
    using ToolkitCreator;

    public class ZombieAttack : ActionWorker
    {
        public override string DisplayName { get; set; }

        public ZombieAction ZombieAction { get; set; } = new ZombieAction(){ count = 1, zombieType = ZombieType.Normal, spawnHowType = SpawnHowType.AllOverTheMap };

        public override void Execute()
        {
            Find.CameraDriver.StartCoroutine(SpawnZombies());
        }

        private IEnumerator SpawnZombies()
        {
            bool spawned = false;
            
            new Notification("Zombies", $"{this.DisplayName} has sent you {ZombieAction.count} {ZombieAction.zombieType} Zombie {(ZombieAction.count > 1? "friends":"friend")}!");

            while (!spawned)
            {       
                if(Current.gameInt == null || Current.gameInt.CurrentMap == null || Current.gameInt.tickManager.Paused)
                    yield return new WaitWhile(()=> Current.gameInt == null || Current.gameInt.CurrentMap == null || Current.gameInt.tickManager.Paused);

				SpawnHowType spawnHowType = ZombieSettings.Values.spawnHowType;
				ZombieSettings.Values.spawnHowType = ZombieAction.spawnHowType;
				spawned = ZombiesRising.TryExecute(Find.CurrentMap, ZombieAction.count, IntVec3.Invalid, false, true, ZombieAction.zombieType);
				ZombieSettings.Values.spawnHowType = spawnHowType;
                yield return null;
            }

            yield break;
        }
    }
}
