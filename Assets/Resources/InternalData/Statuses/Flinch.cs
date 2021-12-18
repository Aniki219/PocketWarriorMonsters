using StatusEffects;
using System.Threading.Tasks;
using UnityEngine;

namespace StatusEffects
{
    public class Flinch : PokemonStatus
    {
        public Flinch()
        {
            duration = 1;
            isVolatile = false;
            buffer = BattleController.BattleBuffer.BATTLE_MOVE;
        }

        public override string ApplyScript()
        {
            return pokemon.displayName + " flinched!";
        }

        public override async Task<BattleMove> DoInterruptStatus(BattleMessageController messageController)
        {
            await base.DoStatus(messageController);
            return new BattleMove(new SkipMove(), null);
        }
    }
}