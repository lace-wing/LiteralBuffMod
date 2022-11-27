using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;
using static LiteralBuffMod.Common.LiteralUtil;
using static LiteralBuffMod.Common.LiteralSets;

namespace LiteralBuffMod.Common
{
    public class LiteralSystem : ModSystem
    {
        /// <summary>
        /// 战斗挑战的持续时间
        /// </summary>
        public static int battleTimer;
        /// <summary>
        /// 战斗挑战的冷却
        /// </summary>
        public static int battleCooldown;
        /// <summary>
        /// 战斗挑战的类型
        /// <para>-1为无, 0为随机选取</para>
        /// <para>1为哥布林挑战</para>
        /// <para>2为雪人挑战</para>
        /// <para>3为海盗挑战</para>
        /// <para>4为火星人挑战</para>
        /// <para>5为血月挑战</para>
        /// <para>6为史莱姆挑战</para>
        /// <para>7为DD2挑战</para>
        /// <para>8为沙尘暴挑战</para>
        /// <para>9为暴风雪挑战</para>
        /// <para>10为日食挑战</para>
        /// <para>11为南瓜月挑战</para>
        /// <para>12为霜月挑战</para>
        /// <para>13为四柱挑战</para>
        /// <para>14为恶意挑战</para>
        /// </summary>
        public static bool[] activeBattle = new bool[15];
        public static int activeBattleCount;

        public static List<Player> battlerPos = new List<Player>();

        /// <summary>
        /// 是史莱姆雨挑战的NPC
        /// </summary>
        public static List<NPC> slimeRainBattleNPC = new List<NPC>();

        public override void PostUpdatePlayers()
        {
            battlerPos.Clear();
            foreach (Player player in Main.player)
            {
                if (player != null && player.active && player.HasBuff(BuffID.Battle))
                {
                    battlerPos.Add(player);
                }
            }
        }

        public override void PreUpdateTime()
        {
            battleTimer = Math.Max(--battleTimer, 0);
            if (battleTimer <= 0)
            {
                battleCooldown = Math.Max(--battleCooldown, 0);
            }
        }

        public override void PreUpdateInvasions()
        {
            Predicate<NPC> npcToRemove = npc => npc == null || !npc.active;
            slimeRainBattleNPC.RemoveAll(npcToRemove);

            activeBattleCount = 0;
            for (int i = 1; i < activeBattle.Length; i++)
            {
                if (activeBattle[i])
                {
                    activeBattleCount++;
                }
            }

            if (activeBattle[0] == true)
            {
                activeBattle[Main.rand.Next(1, 14)] = true;
                activeBattle[0] = false;
            }

            if (activeBattle[1] == true)
            {
                if (battleCooldown <= 0 && Main.dayTime) // 发动挑战
                {
                    battleTimer = 10800;
                    battleCooldown = 600;
                    Main.slimeRain = true;
                }

                if (battleTimer <= 0) // 结束挑战
                {
                    Main.slimeRain = false;
                    activeBattle[1] = false;
                }

                if (battleTimer > 0 && battleTimer % 300 == 0) // 生成NPC
                {
                    foreach (Player battler in battlerPos)
                    {
                        Rectangle white = new Rectangle((int)battler.Center.X - Main.maxScreenW / 3, (int)battler.Center.Y - Main.maxScreenH / 2, Main.maxScreenW * 2 / 3, Main.maxScreenH / 6);
                        Task task = new Task(() =>
                        {
                            NPC[] slimeRainNPCs = TrySpawnNPC(NPC.GetSource_NaturalSpawn(), white, default, Main.hardMode ? hardSlimeRainPool : slimeRainPool);
                            foreach (NPC slime in slimeRainNPCs)
                            {
                                slimeRainBattleNPC.Add(slime);
                            }
                            //slimeRainBattleNPC.AddRange(slimeRainNPCs);
                        });
                        task.Start();
                        task.Wait();
                    }
                }
            }
            for (int i = 2; i < 15; i++)
            {
                if (activeBattle[i])
                {
                    activeBattle[i] = false;
                }
            }
            if (Main.time % 60 == 0)
            {
                Main.NewText($"type: {Main.invasionType}, cType1: {activeBattle[1]}, timer: {battleTimer}, CD: {battleCooldown}, slime; {slimeRainBattleNPC.Count}");
            }
        }
    }
}
