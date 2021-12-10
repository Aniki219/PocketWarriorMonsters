using StatusEffects;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace StatusEffects
{
    public class Confusion : PokemonStatus
    {
        const float CONFUSION_CHANCE = 0.5f;

        public Confusion()
        {
            duration = Random.Range(2, 5);
            isVolatile = true;
            buffer = BattleController.BattleBuffer.BATTLE_MOVE;
        }

        public override async Task<BattleMove> DoInterruptStatus(BattleMessageController messageController)
        {
            await base.DoStatus(messageController);

            string script;

            if (duration > 0)
            {
                script = pokemon.displayName + " is confused...<br><br>";
                await messageController.performScript(script);
                bool hurtSelf = Random.Range(0.0f, 1.0f) < CONFUSION_CHANCE;

                if (hurtSelf)
                {
                    return new BattleMove(new Hitself(pokemon), new List<FieldSlotController> { pokemon.fieldSlot });
                }
                else
                {
                    return null;
                }
            }
            else
            {
                script = pokemon.displayName + " snapped out of confusion!<br><br>";
                await messageController.performScript(script);
                return null;
            }
        }
    }
}