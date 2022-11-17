using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LiteralBuffMod.Common.LiteralUtil;

namespace LiteralBuffMod.Common
{
    internal class LiteralSets
    {
        //internal static bool[] aquaticNPCTypes = new bool[NPCID.Count];
        //internal static int[] vanillaAquaticNPCIds = new int[] { NPCID.AnglerFish, NPCID.DukeFishron, NPCID.EyeballFlyingFish, NPCID.FungoFish, NPCID.FlyingFish, NPCID.BlueJellyfish, NPCID.GreenJellyfish, NPCID.PinkJellyfish, NPCID.IcyMerman, NPCID.ZombieMerman, NPCID.CreatureFromTheDeep };
        internal static HashSet<int> aquaticNPCType = new HashSet<int>()
        {
            //Fish
            NPCID.AnglerFish, NPCID.DukeFishron, NPCID.EyeballFlyingFish, NPCID.FungoFish, NPCID.FlyingFish, 
            //Jellyfish
            NPCID.BlueJellyfish, NPCID.GreenJellyfish, NPCID.PinkJellyfish, 
            //Merman
            NPCID.IcyMerman, NPCID.ZombieMerman, NPCID.CreatureFromTheDeep, 
            //Shark
            NPCID.Shark, NPCID.GoblinShark, NPCID.Sharkron, NPCID.Sharkron2, NPCID.SandShark, NPCID.SandsharkCorrupt, NPCID.SandsharkCrimson, NPCID.SandsharkHallow, 
            //Goldfish
            NPCID.CorruptGoldfish, NPCID.CrimsonGoldfish, NPCID.Goldfish, NPCID.GoldfishWalker, NPCID.GoldGoldfish, NPCID.GoldGoldfishWalker, 
            //Eel
            NPCID.BloodEelHead, NPCID.BloodEelBody, NPCID.BloodEelTail, 
            //Squid
            NPCID.BloodSquid, NPCID.Squid, 
            //Nautilus
            NPCID.BloodNautilus, 
            //Piranha
            NPCID.Piranha, 
            //Sea Snail
            NPCID.SeaSnail, 
            //Crab
            NPCID.Crab, 
            //Crawdad
            NPCID.Crawdad, NPCID.Crawdad2, 
            //Arapaima
            NPCID.Arapaima, 
            //Pigron
            NPCID.PigronCorruption, NPCID.PigronCrimson, NPCID.PigronHallow, 
            //Swamp Thing
            NPCID.SwampThing
        };

        internal static int[] lunarNormalEnemy = new int[] { 402, 406, 407, 409, 411, 412, 415, 416, 417, 418, 419, 420, 421, 423, 424, 425, 428, 429, 518 };
        internal static int[] lunarNormalAmount = new int[] {};
        internal static TrySpawnPool lunarBattlerPool = new TrySpawnPool();

        internal static void SetUpSets()
        {
            //for (int i = 0; i < aquaticNPCTypes.Length; i++)
            //{
            //    aquaticNPCTypes[i] = false;
            //}
            //for (int i = 0; i < vanillaAquaticNPCIds.Length; i++)
            //{
            //    int aqNPC = vanillaAquaticNPCIds[i];
            //    aquaticNPCTypes[aqNPC] = true;
            //}

            lunarBattlerPool.Initialize(lunarNormalEnemy.Length);
            lunarNormalAmount = new int[lunarNormalEnemy.Length];
            for (int i = 0; i < lunarNormalEnemy.Length; i++)
            {
                lunarNormalAmount[i] = 1;
            }
            lunarBattlerPool.Set(true, 30, lunarNormalEnemy, lunarNormalAmount);
        }
    }
}
