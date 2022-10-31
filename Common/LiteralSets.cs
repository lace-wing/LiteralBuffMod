using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteralBuffMod.Common
{
    internal class LiteralSets
    {
        //internal static bool[] aquaticNPCTypes = new bool[NPCID.Count];
        //internal static int[] vanillaAquaticNPCIds = new int[] { NPCID.AnglerFish, NPCID.DukeFishron, NPCID.EyeballFlyingFish, NPCID.FungoFish, NPCID.FlyingFish, NPCID.BlueJellyfish, NPCID.GreenJellyfish, NPCID.PinkJellyfish, NPCID.IcyMerman, NPCID.ZombieMerman, NPCID.CreatureFromTheDeep };
        internal static HashSet<int> aquaticNPCType = new HashSet<int>() { NPCID.AnglerFish, NPCID.DukeFishron, NPCID.EyeballFlyingFish, NPCID.FungoFish, NPCID.FlyingFish, NPCID.BlueJellyfish, NPCID.GreenJellyfish, NPCID.PinkJellyfish, NPCID.IcyMerman, NPCID.ZombieMerman, NPCID.CreatureFromTheDeep };

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
        }
    }
}
