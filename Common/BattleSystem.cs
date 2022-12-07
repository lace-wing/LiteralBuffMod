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
        public static Battle[] Battles = new Battle[] { };
        public static Dictionary<Battle, List<Player>> Battlers = new Dictionary<Battle, List<Player>>();

        public static List<NPC> SlimeRainBattleNPC = new List<NPC>();

        public static bool IsInBattle(Player player)
        {
            foreach (Battle battle in Battlers.Keys)
            {
                if (Battlers[battle].Contains(player) && battle.Active) return true;
            }
            return false;
        }
        public override void PostSetupContent()
        {
            Type[] types = GetType().Assembly.GetTypes();
            Type battleType = typeof(Battle);
            List<Battle> battleList = new List<Battle>();
            foreach (Type type in types)
            {
                if (type.IsSubclassOf(battleType))
                {
                    object obj = Activator.CreateInstance(type);
                    if (obj != null)
                    {
                        Battle aBattle = obj as Battle;
                        battleList.Add(aBattle);
                    }
                }
            }
            Battles = battleList.ToArray();
            for (int i = 0; i < Battles.Length; i++)
            {
                //Battles[i].id = i;
                Battles[i].SetDefaults();
                Battlers.Add(Battles[i], new List<Player>());
            }
        }
        public override void PostUpdatePlayers()
        {
            foreach (Battle battle in Battlers.Keys)
            {
                Battlers[battle].Clear();
            }
            foreach (Player player in Main.player)
            {
                if (player != null && player.active && player.HasBuff(BuffID.Battle))
                {
                    if (!IsInBattle(player))
                    {
                        Battle b = Main.rand.Next(Battlers.Keys.ToArray());
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
    }
}
