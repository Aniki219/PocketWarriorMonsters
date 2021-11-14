using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public FieldSlotController[] allyFieldSlots = new FieldSlotController[3];
    public FieldSlotController[] enemyFieldSlots = new FieldSlotController[3];

    public BattleMessageController BattleMessage;

    //I don't know what this is, but if we use it we should consider a dictionary.
    //That's not a slam I mean the data type.
    //public List<KeyValuePair<GameObject, UnityAction>> buttonActions;

    public enum BattlePhase
    {
        BATTLE_START,
        PLAYER_PLANNING,
        ENEMY_PLANNING,
        BATTLE
    }

    public enum BattleBuffer
    {
        START_BATTLE,
        START_TURN,
        SWAP_POKEMON,
        ITEM,
        PRIORITY_MOVE,
        BATTLE_MOVE,
        TURN_END
    }

    private BattlePhase battlePhase = BattlePhase.BATTLE_START;
    private Dictionary<BattleBuffer, List<BattleAction>> battleBuffer;
    TextGenerator generator;
    TextGenerationSettings settings;
    void Start()
    {
        //We have a bunch of queue types and priorities. This will all be managed by the Enum.
        //Here we make a dictionary of buffers and initialize them all to empty lists.
        //This we we can access each buffer by supplying a buffer type enum.
        battleBuffer = new Dictionary<BattleBuffer, List<BattleAction>>();
        for (int i = 0; i < System.Enum.GetNames(typeof(BattleBuffer)).Length; i++)
        {
            battleBuffer.Add(
                (BattleBuffer)i,
                new List<BattleAction>()
            );
        }

        //Remove this one day
        PlayerPokemon.instance.setPokemonStats();

        sendOutPokemon();

        changeBattlePhase(BattlePhase.PLAYER_PLANNING, 1.5f);
    }

    private IEnumerator changeBattlePhase(BattlePhase phase, float waitTime = 0)
    {
        yield return new WaitForSeconds(waitTime);
        battlePhase = phase;
        //StartCoroutine(startBattlePhase());
    }

    void startBattlePhase()
    {
        switch(battlePhase)
        {
            case BattlePhase.PLAYER_PLANNING:
                Debug.Log("Player Planning Phase");
                //Make battle menu visible
                //Start current pokemon counter
                //Elsewhere
                    //We need an update pokemon counter method which checks if the last pokemon
                    //has selected a BattleAction. Then phase needs to switch
                break;
            case BattlePhase.ENEMY_PLANNING:
                Debug.Log("Enemy Planning Phase");
                break;
            case BattlePhase.BATTLE:
                Debug.Log("Battling Phase...");
                break;
        }
    }

    void Update()
    {
        if (InputManager.getKeyPressed("Space"))
        {
            BattleMessage.GetComponent<Animator>().SetTrigger("Next");
        }
    }

    public async void sendOutPokemon()
    {
        string script = "Wild ";

        //Enemy Pokemon
        int num = 3;
        for (int i = 0; i < num && i < 3; i++)
        {
            int level = Random.Range(1, 100);
            PokemonName randomPokemonName = (PokemonName)(Random.Range(1, 490));
            Pokemon randomPokemon = Pokemon.fromData(PokedexDataReader.getPokemonData(randomPokemonName), level);
            enemyFieldSlots[i].setPokemon(randomPokemon, true);
            enemyFieldSlots[i].pokemonPlaySendIn();

            script += randomPokemonName;
            if (num > 1 && i < num - 1)
            {
                script += ", ";
                if (i == num - 2)
                {
                    script += "and ";
                }
            }
        }
        script += " appeared!<br>";

        await BattleMessage.performScript(script);

        //Player Pokemon
        script = "Atlas sends out ";
        List<Pokemon> playerPokemon = PlayerPokemon.instance.pokemon;
        num = playerPokemon.Count;
        for (int i = 0; i < num && i < 3; i++)
        {
            allyFieldSlots[i].setPokemon(playerPokemon[i], false);
            allyFieldSlots[i].Invoke("playBallAnimation", 0.5f + 0.25f * i);

            script += playerPokemon[i].name;
            if (num > 1 && i < num - 1)
            {
                script += ", ";
                if (i == num - 2)
                {
                    script += "and ";
                }
            }
        }
        script += "!";
        await BattleMessage.performScript(script);
    }

    public void addBattleActionToQueue(BattleAction battleAction, BattleBuffer? overrideBuffer = null)
    {
        /*We can automatically assign the correct buffer by asking the BattleAction which buffer
         * it belongs to. I think this is sufficient but just incase we have an overrideBuffer
         * parameter which can sort out ambiguities
         */
        BattleBuffer bufferEnum = (overrideBuffer.HasValue) ? overrideBuffer.Value : 
            battleAction.getBattleBuffer();

        if (battleAction.Equals(typeof(BattleMove)))
        {
            if (battlePhase.Equals(BattlePhase.PLAYER_PLANNING))
            {
                //Inc player poke counter
            } else
            {
                //Inv enemy poke counter
                //Maybe we dont need this since all enemy pokemon can just add their move at the same
                //time...
            }
        }
        battleBuffer[bufferEnum].Add(battleAction);
    }

    private void clearQueues()
    {
        foreach(KeyValuePair<BattleBuffer, List<BattleAction>> entry in battleBuffer)
        {
            entry.Value.Clear();
        }
    }

    private void clearQueue(BattleBuffer buffer)
    {
        battleBuffer[buffer].Clear();
    }
}
