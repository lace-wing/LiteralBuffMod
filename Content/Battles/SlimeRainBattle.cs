using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using LiteralBuffMod.Common;
using static LiteralBuffMod.Common.LiteralUtil;
using static LiteralBuffMod.Common.LiteralSets;

namespace LiteralBuffMod.Content.Battles
{
    public class SlimeRainBattle : Battle
    {
        public override void SetDefaults()
        {
            Name = "Slime Rain Battle";
            Description = "The slime rain battle";
        }
        public override void OnStartBattle(Player[] players)
        {
            base.OnStartBattle(players);
            Main.NewText($"{Name} begins!", Color.Yellow);
        }
        public override void UpdateWave(Player[] players)
        {
            if (WaveState == State.Starting)
            {
                foreach (Player player in players)
                {
                    Rectangle white = new Rectangle((int)player.Center.X - Main.maxScreenW / 3, (int)player.Center.Y - Main.maxScreenH / 2, Main.maxScreenW * 2 / 3, Main.maxScreenH / 6);
                    Task task = new Task(() =>
                    {
                        NPC[] slimeRainNPCs = SpawnNPCBatch(NPC.GetSource_NaturalSpawn(), white, default, Main.hardMode ? hardSlimeRainPool : slimeRainPool);
                        foreach (NPC slime in slimeRainNPCs)
                        {
                            BattleSystem.SlimeRainBattleNPC.Add(slime);
                        }
                    });
                    task.Start();
                }
                WaveState = State.Progressing;
            }
        }
        public override void PostUpdateWave(Player[] players)
        {
            if (WaveCounter >= 300)
            {
                Wave++;
                WaveState = State.Starting;
                WaveCounter = 0;
            }
            if (Wave == 6)
            {
                BattleState = State.Ending;
            }
        }
    }
}
