using StatusEffects;
using System.Threading.Tasks;
using UnityEngine;

namespace StatusEffects
{
    public class Sleep : PokemonStatus
    {
        public Sleep()
        {
            duration = Random.Range(2,5);
            statusBadgeId = 2;
            isVolatile = false;
            buffer = BattleController.BattleBuffer.BATTLE_MOVE;
        }

        public override string ApplyScript()
        {
            return pokemon.displayName + " fell asleep!";
        }

        public override async Task<BattleMove> DoInterruptStatus(BattleMessageController messageController)
        {
            await base.DoStatus(messageController);

            if (duration > 0)
            {
                string script = "<?zoom|" + pokemon.fieldSlot.slotNumber + ">" + pokemon.displayName + " is fast asleep!<br><br>";
                await messageController.performScript(script);
                return new BattleMove(new SkipMove(), null);
            } else
            {
                string script = "<?zoom|" + pokemon.fieldSlot.slotNumber + ">" + pokemon.displayName + " woke up!";
                await messageController.performScript(script);
                return null;
            }
        }
    }
}