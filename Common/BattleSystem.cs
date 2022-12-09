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

namespace LiteralBuffMod.Common
{
    public class BattleSystem : ModSystem
    {
        public static BaseBattle[] Battles = new BaseBattle[] { };
        public static Dictionary<BaseBattle, List<Player>> Battlers = new Dictionary<BaseBattle, List<Player>>();

        public static List<NPC> SlimeRainBattleNPC = new List<NPC>();

        public static bool IsInBattle(Player player)
        {
            foreach (BaseBattle battle in Battlers.Keys)
            {
                if (Battlers[battle].Contains(player) && battle.Active) return true;
            }
            return false;
        }
        public override void PostSetupContent()
        {
            Type[] types = GetType().Assembly.GetTypes();
            Type battleType = typeof(BaseBattle);
            List<BaseBattle> battleList = new List<BaseBattle>();
            foreach (Type type in types)
            {
                if (type.IsSubclassOf(battleType))
                {
                    object obj = Activator.CreateInstance(type);
                    if (obj != null)
                    {
                        BaseBattle aBattle = obj as BaseBattle;
                        battleList.Add(aBattle);
                    }
                }
            }
            Battles = battleList.ToArray();
            for (int i = 0; i < Battles.Length; i++)
            {
                Battles[i].SetDefaults();
                Battlers.Add(Battles[i], new List<Player>());
            }
        }
        public override void PostUpdatePlayers()
        {
            foreach (BaseBattle battle in Battlers.Keys)
            {
                Battlers[battle].Clear();
            }
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
        public override void PreUpdateInvasions()
        {
            for (int i = 0; i < Battles.Length; i++)
            {
                Battles[i].UpdateBattle(Battlers[Battles[i]].ToArray());
            }
        }
        public override void PostUpdateNPCs()
        {
            SlimeRainBattleNPC.RemoveAll(match => match == null || !match.active);
        }
    }
}
