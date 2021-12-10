using StatusEffects;
using System.Threading.Tasks;
using UnityEngine;

namespace StatusEffects
{
    public class Burn : PokemonStatus
    {
        const float BURN_PERCENT = 0.5f;
        const float BURN_DAMAGE_RATIO = 1.0f / 8.0f;

        public Burn()
        {
            duration = -1;
            isVolatile = false;
            buffer = BattleController.BattleBuffer.TURN_END;
        }

        public override async Task DoStatus(BattleMessageController messageController)
        {
            await base.DoStatus(messageController);

            int damage = Mathf.RoundToInt(pokemon.getStatValue(Stats.HP) * BURN_DAMAGE_RATIO);
            string script = pokemon.displayName + " takes " + damage + " damage from their burn!<br>";
            await messageController.performScript(script);
            await pokemon.fieldSlot.takeDamage(damage);

            return;
        }
    }
}