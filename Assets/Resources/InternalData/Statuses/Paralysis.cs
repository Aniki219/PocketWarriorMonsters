using StatusEffects;
using System.Threading.Tasks;
using UnityEngine;

namespace StatusEffects
{
    public class Paralysis : PokemonStatus
    {
        private const float PARALYSIS_CHANCE = 0.25f;
        public Paralysis()
        {
            duration = -1;
            isVolatile = false;
            buffer = BattleController.BattleBuffer.BATTLE_MOVE;
        }

        public override async Task<BattleMove> DoInterruptStatus(BattleMessageController messageController)
        {
            await base.DoStatus(messageController);

            float rand = Random.Range(0.0f, 1.0f);
            bool isParalyzed = rand < PARALYSIS_CHANCE;
            Debug.Log("parachance: " + rand);
            if (isParalyzed)
            {
                string script = "<?zoom|" + pokemon.fieldSlot.slotNumber + ">" + pokemon.displayName + " is fully paralyzed!<br><br>";
                await messageController.performScript(script);
                return new BattleMove(new SkipMove(), null);
            }
            return null;
        }
    }
}