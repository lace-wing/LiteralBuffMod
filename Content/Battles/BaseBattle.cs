using IL.Terraria.GameContent.Bestiary;
using LiteralBuffMod.Common;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Default;

namespace LiteralBuffMod.Content.Battles
{
    public enum State
    {
        Starting, 
        Progressing, 
        Ending
    }
    public abstract class Battle
    {
        /// <summary>
        /// Name of the battle
        /// </summary>
        public string Name = string.Empty;
        /// <summary>
        /// Description of the battle
        /// </summary>
        public string Description = string.Empty;
        
        /// <summary>
        /// Controls whether the methods in UpdateBattle will be called or not
        /// </summary>
        public bool Active = false;
        /// <summary>
        /// State of the battle
        /// </summary>
        public State BattleState = State.Starting;
        /// <summary>
        /// +1 after PostUpdateWave when the battle is Progressing
        /// </summary>
        public int BattleCounter = 0;
        /// <summary>
        /// Current level
        /// </summary>
        public int Level = 0;
        /// <summary>
        /// Current wave number
        /// </summary>
        public int Wave = 0;
        /// <summary>
        /// State of the current wave
        /// </summary>
        public State WaveState = State.Starting;
        /// <summary>
        /// +1 before PostUpdateWave when the battle is Progressing
        /// </summary>
        public int WaveCounter = 0;
        /// <summary>
        /// Counters which can be used for any purpose
        /// </summary>
        public float[] Counter = new float[3];

        /// <summary>
        /// Called in PostSetupContent
        /// </summary>
        public virtual void SetDefaults()
        {
            Name = string.Empty;
            Description = string.Empty;
        }
        /// <summary>
        /// Update the battle
        /// <br>Called in PreUpdateInvasions, which means it won't be called on MP client</br>
        /// <br>Can be manually called for extra updates</br>
        /// </summary>
        /// <param name="battlers">Players in this battle</param>
        public void UpdateBattle(Player[] battlers)
        {
            if (Active)
            {
                if (BattleState == State.Starting)
                {
                    OnStartBattle(battlers);
                }
                if (BattleState == State.Progressing)
                {
                    if (PreUpdateWave(battlers))
                    {
                        UpdateWave(battlers);
                    }
                    WaveCounter = Math.Max(++WaveCounter, 0);
                    PostUpdateWave(battlers);
                    BattleCounter = Math.Max(++WaveCounter, 0);
                }
                if (BattleState == State.Ending)
                {
                    OnEndBattle(battlers);
                }
            }
        }
        /// <summary>
        /// Called only if the battle is Starting
        /// <br>The base function resets the battle and proceeds to Progressing</br>
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void OnStartBattle(Player[] players)
        {
            ResetBattle(true);
            BattleState = State.Progressing;
        }
        /// <summary>
        /// Called only if the battle is Ending
        /// <br>The base function resets the battle and deactivates it</br>
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void OnEndBattle(Player[] players)
        {
            ResetBattle();
        }
        /// <summary>
        /// Called before UpdateWave when the battle is Progressing
        /// </summary>
        /// <param name="players">Players in this battle</param>
        /// <returns>true to allow UpdateWave to be called, true by default</returns>
        public virtual bool PreUpdateWave(Player[] players) { return true; }
        /// <summary>
        /// Called after PostUpdateWave and before UpdateWave when the battle is Progressing
        /// <br>Won't be called if PreUpdateWave returns false</br>
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void UpdateWave(Player[] players) { }
        /// <summary>
        /// Called after UpdateWave and after counting up WaveCounter when the battle is Progressing regardless of PreUpdateWave
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void PostUpdateWave(Player[] players) { }

        /// <summary>
        /// Starts the battle with a certain Level and Wave
        /// </summary>
        /// <param name="level"></param>
        /// <param name="wave"></param>
        public void StartBattle(int level = 0, int wave = 0)
        {
            Active = true;
            BattleState = State.Starting;
            Level = level;
            Wave = wave;
            WaveState = State.Starting;
        }
        /// <summary>
        /// Resets the battle. Level, Wave and all counters are set to 0, all states are set to Starting
        /// </summary>
        /// <param name="active">What Active is set to</param>
        public void ResetBattle(bool active = false)
        {
            Active = active;
            BattleState = State.Starting;
            BattleCounter = 0;
            Level = 0;
            Wave = 0;
            WaveState = State.Starting;
            WaveCounter = 0;
            for (int i = 0; i < Counter.Length; i++)
            {
                Counter[i] = 0;
            }
        }
        /// <summary>
        /// Resets the wave. WaveCounter is set to 0 and WaveState is set to Starting
        /// </summary>
        /// <param name="wave">Waht Wave is set to</param>
        public void ResetWave(int wave = 0)
        {
            Wave = wave;
            WaveState = State.Starting;
            WaveCounter = 0;
        }
    }
}
