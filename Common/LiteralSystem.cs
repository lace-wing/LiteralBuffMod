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
        public static int battleChallengeTimer;
        /// <summary>
        /// 战斗挑战的冷却
        /// </summary>
        public static int battleChallengeCooldown;
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
        public static int battleChallengeType = -1;
        public static List<Vector2> battlerPos = new List<Vector2>();

        public override void PostUpdatePlayers()
        {
            battlerPos.Clear();
            foreach (Player player in Main.player)
            {
                if (player != null && player.active && player.HasBuff(BuffID.Battle))
                {
                    battlerPos.Add(player.Center);
                }
            }
        }
        public override void PreUpdateTime()
        {
            battleChallengeTimer = Math.Max(--battleChallengeTimer, 0);
            if (battleChallengeTimer <= 0)
            {
                battleChallengeCooldown = Math.Max(--battleChallengeCooldown, 0);
            }
        }

        public override void PreUpdateInvasions()
        {
            switch (battleChallengeType)
            {
                case -1 : break;
                case 0 :
                    {
                        battleChallengeType = Main.rand.Next(1, 14);
                        break;
                    }
                case 1 :
                    {
                        if (battleChallengeCooldown <=0 && Main.dayTime)
                        {
                            battleChallengeTimer = 3600;
                            battleChallengeCooldown = 300;
                            Main.slimeRain = true;
                            slimeRainPool.totalAmount = 3;
                            hardSlimeRainPool.totalAmount = 3;
                        }
                        if (battleChallengeTimer % 120 == 0)
                        {
                            foreach (Vector2 center in battlerPos)
                            {
                                Rectangle white = new Rectangle((int)center.X - Main.maxScreenW / 3, (int)center.Y - Main.maxScreenH / 2, Main.maxScreenW * 2 / 3, Main.maxScreenH / 6);
                                Task task = new Task(() =>
                                {
                                    TrySpawnResult result = TrySpawnNPC(NPC.GetSource_NaturalSpawn(), white, default, Main.hardMode ? hardSlimeRainPool : slimeRainPool);
                                    Main.NewText(result.totalAmount);
                                });
                                task.Start();
                            }
                        }
                        if (battleChallengeTimer <= 0)
                        {
                            Main.slimeRain = false;
                            battleChallengeType = -1;
                        }
                        break;
                    }
                case 2 :
                case 3 :
                case 4 :
                case 5 :
                case 6 : 
                case 7 :
                case 8 :
                case 9 :
                case 10 :
                case 11 :
                case 12 :
                case 13 :
                case 14 :
                default :
                    {
                        battleChallengeType = -1;
                        break;
                    }
            }
            if (Main.time % 60 == 0)
                Main.NewText($"type: {Main.invasionType}, cType: {battleChallengeType}, timer: {battleChallengeTimer}, CD: {battleChallengeCooldown}");
        }
    }
}
