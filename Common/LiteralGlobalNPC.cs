using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteralBuffMod.Common
{
    public class LiteralGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int npcActiveTime;

        public bool npcDrown;

        public override void PostAI(NPC npc)
        {
            if (npcActiveTime >= int.MaxValue || npcActiveTime < 0)
            {
                npcActiveTime = 0;
            }
            npcActiveTime++;

            if (npc.type == NPCID.IcyMerman || npc.type == NPCID.ZombieMerman || npc.type == NPCID.CreatureFromTheDeep) // 水生生物离水窒息
            {
                if (!npc.wet)
                {
                    if (npc.breath >= 0 && npcActiveTime % 7 == 0)
                    {
                        npc.breath--;
                    }
                    if (npc.breath <= 0)
                    {
                        npc.breath = 0;
                        npcDrown = true;
                    }
                }
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npcDrown) // 溺水掉血
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 4;
                if (npc.lifeRegen <= 0)
                {
                    npc.life = 1;
                    npc.StrikeNPCNoInteraction(4, 0f, 0);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, 2f);
                    }
                }
            }
        }
    }
}
