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
using Terraria.Localization;

namespace LiteralBuffMod.Content.Battles
{
    public class SlimeRainBattle : BaseBattle
    {
        private string bKey = "Mods.LiteralBuffMod.Battle.";
        public override void SetDefaults()
        {
            Name = "Slime Rain Battle";
            Description = "The slime rain battle";
            MaxWave = 5;
            Delay = 360;
        }
        public override void OnBattleStart(Player[] players)
        {
            Main.NewText(Language.GetTextValue(bKey + "BattleStart", Name), Color.Yellow);
            foreach (Player player in players)
            {
                if (!player.dead)
                {
                    player.QuickSpawnItem(player.GetSource_GiftOrReward("SlimeRainBattle"), ItemID.Heart);
                    player.QuickSpawnItem(player.GetSource_GiftOrReward("SlimeRainBattle"), ItemID.Star);
                    player.AddBuff(BuffID.Slimed, 300);
                }
            }
        }
        public override void OnBattleEnd(Player[] players)
        {
            Main.NewText($"{Name} ends!", Color.Yellow);
            if (BattleSystem.SlimeRainBattleNPC.Count <= 0)
            {
                foreach (Player player in players)
                {
                    if (!player.dead)
                    {
                        player.QuickSpawnItem(player.GetSource_GiftOrReward("SlimeRainBattle"), ItemID.GoldCoin, Main.hardMode ? 60 : 30);
                        CombatText.NewText(player.Hitbox, Color.Yellow, Language.GetTextValue(bKey + "SlimeRain.Success"));
                    }
                }
            }
            else
            {
                foreach (Player player in players)
                {
                    player.QuickSpawnItem(player.GetSource_GiftOrReward("SlimeRainBattle"), ItemID.AngelStatue);
                    CombatText.NewText(player.Hitbox, Color.Yellow, Language.GetTextValue(bKey + "SlimeRain.Failure"));
                }
            }
            foreach (NPC npc in BattleSystem.SlimeRainBattleNPC)
            {
                npc.active = false;
            }
            Delay = 120;
        }
        public override void OnWaveStart(Player[] players)
        {
            Main.NewText(Language.GetTextValue(bKey + "WaveStart", Wave + 1), Color.Yellow);
            switch (Wave)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    MaxWaveTimer = 720;
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
                    MaxWaveTimer = 1200;
                    foreach (Player player in players)
                    {
                        NPC npc = NPC.NewNPCDirect(NPC.GetBossSpawnSource(player.whoAmI), player.Center + new Vector2(0, -360), NPCID.KingSlime);
                        BattleSystem.SlimeRainBattleNPC.Add(npc);
                    }
                    break;
            }
        }
        public override void InWave(Player[] players)
        {
            if (BattleSystem.SlimeRainBattleNPC.Count <= 0 && WaveTimer > 120)
            {
                WaveState = State.Ending;
            }
        }
        public override void OnWaveEnd(Player[] players)
        {
            foreach (Player player in players)
            {
                switch (Wave)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        for (int i = 0; i < Wave; i++)
                        {
                            player.QuickSpawnItem(player.GetSource_GiftOrReward("SlimeRainBattle"), ItemID.Heart);
                        }
                        break;
                }
            }
        }
    }
}
