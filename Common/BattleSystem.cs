using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using LiteralBuffMod.Content.Battles;
using static LiteralBuffMod.Content.Battles.BaseBattleSystem;

namespace LiteralBuffMod.Common
{
    public class BattleSystem : ModSystem
    {
        public static List<NPC> SlimeRainBattleNPC = new List<NPC>();

        public override void PostUpdatePlayers()
        {
            foreach (Player player in Main.player)
            {
                if (player != null && player.active && player.HasBuff(BuffID.Battle))
                {
                    if (!IsInBattle(player))
                    {
                        BaseBattle b = Main.rand.Next(Battlers.Keys.ToArray());
                        Battlers[b].Add(player);
                        b.TryStartBattle();
                    }
                }
            }
        }
    }
}
