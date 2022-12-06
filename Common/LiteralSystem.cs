using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;
using static LiteralBuffMod.Common.LiteralUtil;
using static LiteralBuffMod.Common.LiteralSets;
using LiteralBuffMod.Content;
using LiteralBuffMod.Content.Battles;

namespace LiteralBuffMod.Common
{
    /*public class LiteralSystem : ModSystem
    {
        /// <summary>
        /// 战斗挑战的冷却
        /// </summary>
        public static int[] battleCooldown = new int[15];
        /// <summary>
        /// 战斗挑战的类型
        /// <para>0为总控</para>
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
        public static int[] battleTimer = new int[15];
        public static int activeBattleCount;
        public static int[] waveCount = new int[15];
        public static int[] waveInterval = new int[15];

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

        public override void PostUpdateEverything()
        {
            for (int i = 0; i < battleTimer.Length; i++)
            {
                battleTimer[i] = Math.Max(--battleTimer[i], 0);
                if (battleTimer[i] <= 0)
                {
                    battleCooldown[i] = Math.Max(--battleCooldown[i], 0);
                }
            }
        }

        public override void PreUpdateInvasions()
        {
            #region Settle battle challenge gields
            Predicate<NPC> npcToRemove = npc => npc == null || !npc.active;
            slimeRainBattleNPC.RemoveAll(npcToRemove);

            activeBattleCount = 0;
            for (int i = 1; i < battleTimer.Length; i++)
            {
                if (battleTimer[i] > 0)
                {
                    activeBattleCount++;
                }
            }
            #endregion

            if (battleTimer[0] > 0)
            {
                if (battleTimer[1] > 0)
                {
                    if (battleCooldown <= 0 && Main.dayTime) // 发动挑战
                    {
                        battleTimer = 10800;
                        battleCooldown = 600;
                        Main.slimeRain = true;
                    }

                    if (battleTimer <= 0 || (waveCount[1] >= 9 && slimeRainBattleNPC.Count <= 0)) // 结束挑战
                    {
                        Main.slimeRain = false;
                        battleTimer[1] = false;
                    }

                    if (battleTimer > 0 && battleTimer % 300 == 0) // 生成NPC
                    {
                        foreach (Player battler in battlerPos)
                        {
                            Rectangle white = new Rectangle((int)battler.Center.X - Main.maxScreenW / 3, (int)battler.Center.Y - Main.maxScreenH / 2, Main.maxScreenW * 2 / 3, Main.maxScreenH / 6);
                            Task task = new Task(() =>
                            {
                                NPC[] slimeRainNPCs = SpawnNPCBatch(NPC.GetSource_NaturalSpawn(), white, default, Main.hardMode ? hardSlimeRainPool : slimeRainPool);
                                foreach (NPC slime in slimeRainNPCs)
                                {
                                    slimeRainBattleNPC.Add(slime);
                                }
                            });
                            task.Start();
                            task.Wait();
                        }
                    }
                }
                for (int i = 2; i < 15; i++)
                {
                    if (battleTimer[i])
                    {
                        battleTimer[i] = false;
                    }
                }
            }

            if (Main.time % 60 == 0)
            {
                Main.NewText($"type: {Main.invasionType}, cType1: {battleTimer[1]}, timer: {battleTimer}, CD: {battleCooldown}, slime; {slimeRainBattleNPC.Count}");
            }
        }
    }*/
}
