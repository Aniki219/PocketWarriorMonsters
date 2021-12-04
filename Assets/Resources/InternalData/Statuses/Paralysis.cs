﻿
using System.Threading.Tasks;
using UnityEngine;

public class Paralysis : PokemonStatus
{
    private const float PARALYSIS_CHANCE = 0.25f;
    public Paralysis(Pokemon pokemon) :  base(pokemon)
    {
        duration = -1;
        secondary = false;
        buffer = BattleController.BattleBuffer.BATTLE_MOVE;
    }

    public override async Task<bool> DoStatus(BattleMessageController messageController)
    {
        await base.DoStatus(messageController);

        float rand = Random.Range(0.0f, 1.0f);
        bool isParalyzed = rand < PARALYSIS_CHANCE;

        if (isParalyzed)
        {
            string script = "<?zoom|" + pokemon.fieldSlot.slotNumber + ">" + pokemon.displayName + " is fully paralyzed!<br><br>";
            await messageController.performScript(script);
        }
        return isParalyzed;
    }
}

