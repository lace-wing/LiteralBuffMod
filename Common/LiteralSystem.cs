using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteralBuffMod.Common
{
    public class LiteralSystem : ModSystem
    {
        public bool noInvasion;
        public bool noMoonEvent;
        public bool noLunarEvent;

        public override void PreUpdateTime()
        {
            noInvasion = false;
            noMoonEvent = false;
            noLunarEvent = false;

            if (Main.invasionType == 0)
            {
                noInvasion = true;
            }
            if (!Main.bloodMoon && !Main.pumpkinMoon && !Main.snowMoon)
            {
                noMoonEvent = true;
            }
        }
    }
}
