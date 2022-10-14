using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteralBuffMod.Common
{
    public class LiteralGlobalBuff : GlobalBuff
    {
        public override bool ReApply(int type, Player player, int time, int buffIndex)
        {
            return base.ReApply(type, player, time, buffIndex);
        }
        public override void Update(int type, Player player, ref int buffIndex)
        {
            base.Update(type, player, ref buffIndex);
        }
    }
}
