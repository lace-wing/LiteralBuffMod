using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;
using LiteralBuffMod.Common;
using static LiteralBuffMod.Common.LiteralUtil;
using static LiteralBuffMod.Common.LiteralSets;
using Microsoft.Xna.Framework.Content;

namespace LiteralBuffMod.Content.Battles
{
    public class SlimeRainBattle : Battle
    {
        public override void SetDefaults()
        {
            Name = "Slime Rain Battle";
            Description = "The slime rain battle";
            MaxWave = 5;
            Delay = 360;
        }
        public override void OnStartBattle(Player[] players)
        {
            Main.NewText($"{Name} begins!", Color.Yellow);
        }
        public override void OnEndBattle(Player[] players)
        {
            Main.NewText($"{Name} ends!", Color.Yellow);
        }
        public override void OnStartWave(Player[] players)
        {
            Main.NewText("Wave " + Wave + " starts!", Color.Yellow);
            switch (Wave)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    MaxWaveCounter = 3600;
                    foreach (Player player in players)
                    {
                        Rectangle white = new Rectangle((int)player.Center.X - Main.maxScreenW / 3, (int)player.Center.Y - Main.maxScreenH / 2, Main.maxScreenW * 2 / 3, Main.maxScreenH / 6);
                        NPC[] slimeRainNPCs = SpawnNPCBatch(NPC.GetSource_NaturalSpawn(), white, default, Main.hardMode ? hardSlimeRainPool : slimeRainPool);
                        foreach (NPC slime in slimeRainNPCs)
                        {
                            BattleSystem.SlimeRainBattleNPC.Add(slime);
                        }
                    }
                    break;
                case 5:
                    MaxWaveCounter = 1200;
                    foreach (Player player in players)
                    {
                        NPC npc = NPC.NewNPCDirect(NPC.GetBossSpawnSource(player.whoAmI), player.Center + new Vector2(0, -320), NPCID.KingSlime);
                        BattleSystem.SlimeRainBattleNPC.Add(npc);
                    }
                    break;
            }
        }
        public override void InWave(Player[] players)
        {
            if (BattleSystem.SlimeRainBattleNPC.Count == 0)
            {
                ResetWave(++Wave);
            }
        }
    }
}
