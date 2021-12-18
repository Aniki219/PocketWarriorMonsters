using StatusEffects;
using System.Threading.Tasks;
using UnityEngine;

namespace StatusEffects
{
    public class Freeze : PokemonStatus
    {
        private const float THAW_CHANCE = 0.2f;

        public Freeze()
        {
            duration = -1;
            statusBadgeId = 3;
            isVolatile = false;
            buffer = BattleController.BattleBuffer.BATTLE_MOVE;
        }

        public override string ApplyScript()
        {
            return pokemon.displayName + " was frozen solid!";
        }

        public override async Task<BattleMove> DoInterruptStatus(BattleMessageController messageController)
        {
            await base.DoStatus(messageController);

            bool didThaw = Random.Range(0.0f, 1.0f) < THAW_CHANCE;

            if (!didThaw)
            {
                string script = "<?zoom|" + pokemon.fieldSlot.slotNumber + ">" + pokemon.displayName + " is still frozen!<br><br>";
                await messageController.performScript(script);
                return new BattleMove(new SkipMove(), null);
            } else
            {
                string script = "<?zoom|" + pokemon.fieldSlot.slotNumber + ">" + pokemon.displayName + " thawed out!";
                await messageController.performScript(script);
                return null;
            }
        }

        public async Task unfreeze(BattleMessageController messageController)
        {
            duration = 0;
            string script = "<?zoom|" + pokemon.fieldSlot.slotNumber + ">" + pokemon.displayName + " thawed out!";
            await messageController.performScript(script);
        }
    }
}