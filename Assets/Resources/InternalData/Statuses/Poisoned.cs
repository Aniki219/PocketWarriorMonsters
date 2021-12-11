using StatusEffects;
using System.Threading.Tasks;
using UnityEngine;

namespace StatusEffects
{
    public class Poisoned : PokemonStatus
    {
        const float POISON_RATIO = 1.0f / 8.0f;

        public Poisoned()
        {
            duration = -1;
            isVolatile = false;
            buffer = BattleController.BattleBuffer.TURN_END;
        }

        public override string ApplyScript()
        {
            return pokemon.displayName + " was poisoned!<br>";
        }

        public override async Task DoStatus(BattleMessageController messageController)
        {
            await base.DoStatus(messageController);

            int damage = Mathf.RoundToInt(pokemon.getStatValue(Stats.HP) * POISON_RATIO);
            string script = pokemon.displayName + " takes " + damage + " damage from poison!<br>";
            await messageController.performScript(script);
            await pokemon.fieldSlot.takeDamage(damage);
        }
    }
}