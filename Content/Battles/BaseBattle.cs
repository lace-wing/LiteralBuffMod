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
    public abstract class Battle // TODO 属性
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
        public int Delay = 0;
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
        /// Battle ends if Wave > MaxWave
        /// </summary>
        public int MaxWave = 0;
        /// <summary>
        /// State of the current wave
        /// </summary>
        public State WaveState = State.Starting;
        /// <summary>
        /// +1 before PostUpdateWave when the battle is Progressing
        /// </summary>
        public int WaveCounter = 0;
        /// <summary>
        /// Wave ends if WaveCounter > MaxWaveCounter
        /// </summary>
        public int MaxWaveCounter = 0;
        /// <summary>
        /// Counters which can be used for any purpose
        /// </summary>
        public float[] Counter = new float[4];

        /// <summary>
        /// Called in PostSetupContent
        /// </summary>
        public virtual void SetDefaults()
        {
            Name = string.Empty;
            Description = string.Empty;
            MaxWave = 1;
            MaxWaveCounter = 60;
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
                    ResetBattle(true);
                    OnStartBattle(battlers);
                    BattleState = State.Progressing;
                }
                if (BattleState == State.Progressing)
                {
                    if (PreUpdateWave(battlers))
                    {
                        if (WaveState == State.Starting)
                        {
                            OnStartWave(battlers);
                            WaveState = State.Progressing;
                        }
                        if (WaveState == State.Progressing)
                        {
                            InWave(battlers);
                            WaveCounter = Math.Max(++WaveCounter, 0);
                            if (MaxWaveCounter >= 0 && WaveCounter > MaxWaveCounter)
                            {
                                WaveCounter = 0;
                                WaveState = State.Ending;
                            }
                        }
                        if (WaveState == State.Ending)
                        {
                            OnEndWave(battlers);
                            ResetWave(++Wave);
                        }
                    }
                    PostUpdateWave(battlers);
                    if (MaxWave >= 0 && Wave > MaxWave)
                    {
                        BattleState = State.Ending;
                    }
                    BattleCounter = Math.Max(++BattleCounter, 0);
                }
                if (BattleState == State.Ending)
                {
                    OnEndBattle(battlers);
                    ResetBattle();
                }
            }
            else
            {
                ResetBattle();
            }
            Delay = Math.Max(--Delay, 0);
        }
        /// <summary>
        /// Called only if the battle is Starting
        /// <br>The base function resets the battle and proceeds to Progressing</br>
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void OnStartBattle(Player[] players) { }
        /// Called only if the battle is Ending
        /// <br>The base function resets the battle and deactivates it</br>
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void OnEndBattle(Player[] players) { }
        /// <summary>
        /// Called before UpdateWave when the battle is Progressing
        /// </summary>
        /// <param name="players">Players in this battle</param>
        /// <returns>true to allow UpdateWave to be called, true by default</returns>
        public virtual bool PreUpdateWave(Player[] players) { return true; }
        /// <summary>
        /// Called after PreUpdateWave and before PostUpdateWave when the battle is Progressingand the Wave is Starting
        /// <br>Won't be called if PreUpdateWave returns false</br>
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void OnStartWave(Player[] players) { }
        /// <summary>
        /// Called after PreUpdateWave and before PostUpdateWave when the battle is Progressing and the Wave is Progressing
        /// <br>Won't be called if PreUpdateWave returns false</br>
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void InWave(Player[] players) { }
        /// <summary>
        /// Called after PreUpdateWave and before PostUpdateWave when the battle is Progressingand the Wave is Ending
        /// <br>Won't be called if PreUpdateWave returns false</br>
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void OnEndWave(Player[] players) { }
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
        public bool TryStartBattle(int level = 0, int wave = 0)
        {
            if (!Active && Delay <= 0)
            {
                Active = true;
                BattleState = State.Starting;
                Level = level;
                Wave = wave;
                WaveState = State.Starting;
                return true;
            }
            return false;
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
