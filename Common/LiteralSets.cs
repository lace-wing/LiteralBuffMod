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
        internal static List<int> aquaticNPCTypes = new List<int>();

        internal static void SetUpSets()
        {
            aquaticNPCTypes = new List<int>() { NPCID.AnglerFish, NPCID.DukeFishron, NPCID.EyeballFlyingFish, NPCID.FungoFish, NPCID.FlyingFish, NPCID.BlueJellyfish, NPCID.GreenJellyfish, NPCID.PinkJellyfish, NPCID.IcyMerman, NPCID.ZombieMerman, NPCID.CreatureFromTheDeep };
        }
    }
}
