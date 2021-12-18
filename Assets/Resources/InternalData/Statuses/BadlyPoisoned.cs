using StatusEffects;
using System.Threading.Tasks;
using UnityEngine;

namespace StatusEffects
{
    public class BadlyPoisoned : PokemonStatus
    {
        const float BADLY_POISONED_RATIO = 1.0f / 16.0f;

        int tick = 1;

        public BadlyPoisoned()
        {
            duration = -1;
            statusBadgeId = 0;
            isVolatile = false;
            buffer = BattleController.BattleBuffer.TURN_END;
        }

        public override string ApplyScript()
        {
            return pokemon.displayName + " was badly poisoned!<br>";
        }

        public override async Task DoStatus(BattleMessageController messageController)
        {
            await base.DoStatus(messageController);

            int damage = Mathf.RoundToInt(pokemon.getStatValue(Stats.HP) * BADLY_POISONED_RATIO * tick);
            tick++;
            string script = pokemon.displayName + " takes " + damage + " damage from poison!<br>";
            await messageController.performScript(script);
            await pokemon.fieldSlot.takeDamage(damage);
        }
    }
}