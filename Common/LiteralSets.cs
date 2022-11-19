using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LiteralBuffMod.Common.LiteralUtil;
using IL.Terraria.GameContent.NetModules;

namespace LiteralBuffMod.Common
{
    internal class LiteralSets
    {
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


        internal static int[] slimeRainEnemy = new int[] 
        {
            // Slime rain slimes
            NPCID.GreenSlime, NPCID.BlueSlime, NPCID.PurpleSlime, NPCID.Pinky,
            // Other weak slimes
            NPCID.RedSlime, NPCID.YellowSlime, NPCID.BlackSlime, NPCID.IceSlime, NPCID.SandSlime, NPCID.UmbrellaSlime, 
            // Other string slimes
            NPCID.JungleSlime, NPCID.SpikedIceSlime, NPCID.SpikedJungleSlime, NPCID.MotherSlime, 
            // King Slime's summons
            NPCID.SlimeSpiked, 
            // Treasure slimes
            NPCID.DungeonSlime, NPCID.GoldenSlime
        };
        internal static int[] slimeRainAmount = new int[]
        {
            3, 3, 2, 2, 
            2, 2, 2, 2, 2, 2, 
            1, 1, 1, 1, 
            2, 
            1, 1
        };
        internal static int[] hardSlimeRainEnemy = new int[]
        {
            // Lava Slime
            NPCID.LavaSlime, 
            // Post WoF slimes
            NPCID.ToxicSludge, NPCID.CorruptSlime, NPCID.Crimslime, NPCID.Slimer, NPCID.Gastropod, NPCID.IlluminantSlime, NPCID.HoppinJack, 
            // Queen Slime's summons
            NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple, 
            // Treasure slimes
            NPCID.RainbowSlime
        };
        internal static int[] hardSlimeRainAmount = new int[]
        {
            1, 
            2, 2, 2, 3, 3, 2, 1, 
            1, 1, 1, 
            1
        };
        internal static TrySpawnPool slimeRainPool = new TrySpawnPool();
        internal static TrySpawnPool hardSlimeRainPool = new TrySpawnPool();

        internal static void SetUpSets()
        {
            lunarBattlerPool.Initialize(lunarNormalEnemy.Length);
            lunarNormalAmount = new int[lunarNormalEnemy.Length];
            for (int i = 0; i < lunarNormalEnemy.Length; i++)
            {
                lunarNormalAmount[i] = 1;
            }
            lunarBattlerPool.Set(true, 6, lunarNormalEnemy, lunarNormalAmount);

            slimeRainPool.Initialize(slimeRainEnemy.Length);
            slimeRainPool.Set(true, 6, slimeRainEnemy, slimeRainAmount);
            hardSlimeRainPool.Initialize(slimeRainEnemy.Length + hardSlimeRainEnemy.Length);
            hardSlimeRainPool.Set(true, 6, slimeRainEnemy.Concat(hardSlimeRainEnemy).ToArray(), slimeRainAmount.Concat(hardSlimeRainAmount).ToArray());
        }
    }
}
