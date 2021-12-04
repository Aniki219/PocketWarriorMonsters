using System.Threading.Tasks;
using UnityEngine;

public class Confusion : PokemonStatus
{
    const float CONFUSION_CHANCE = 0.5f;

    public Confusion(Pokemon pokemon) : base(pokemon)
    {
        duration = Random.Range(2, 5);
        secondary = true;
        buffer = BattleController.BattleBuffer.BATTLE_MOVE;
    }

    public override async Task<bool> DoStatus(BattleMessageController messageController)
    {
        await base.DoStatus(messageController);

        string script;

        if (duration > 0)
        {
            script = pokemon.displayName + " is confused...<br><br>";
            await messageController.performScript(script);
            bool hurtSelf = Random.Range(0.0f, 1.0f) < CONFUSION_CHANCE;
            return hurtSelf;
        } else
        {
            script = pokemon.displayName + " snapped out of confusion!<br><br>";
            await messageController.performScript(script);
            return false;
        }
    }
}

