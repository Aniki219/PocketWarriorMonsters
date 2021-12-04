using System.Threading.Tasks;
using UnityEngine;

public class Burn : PokemonStatus
{
    const float BURN_PERCENT = 0.5f;

    public Burn(Pokemon pokemon) : base(pokemon)
    {
        duration = -1;
        secondary = false;
        buffer = BattleController.BattleBuffer.TURN_END;
    }

    public override async Task<bool> DoStatus(BattleMessageController messageController)
    {
        await base.DoStatus(messageController);

        string script = pokemon.displayName + " is hurt by its burn!<br><br>";
        await messageController.performScript(script);
        return true;
    }
}

