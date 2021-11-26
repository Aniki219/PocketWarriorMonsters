using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public List<FieldSlotController> allyFieldSlots = new List<FieldSlotController>();
    public List<FieldSlotController> enemyFieldSlots = new List<FieldSlotController>();

    public BattleMessageController BattleMessage;
    public BattleMenuController BattleMenu;

    private bool battleOver;

    public static int tick = 0;
    public static int currentPokemonIndex = 0;

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

    private BattlePhase battlePhase;
    private Dictionary<BattleBuffer, List<BattleAction>> battleBuffer;
    TextGenerator generator;
    TextGenerationSettings settings;
    void Start()
    {
        TwoClock();
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

        //Entry point for the battle scene
        changeBattlePhase(BattlePhase.BATTLE_START);
    }

    /* This is the setup method for each battle phase.
     * the doBattle method acts as the update method */
    private void changeBattlePhase(BattlePhase phase)
    {
        battlePhase = phase;
        doBattle();
    }

    private async void doBattle()
    {
        switch(battlePhase)
        {
            case BattlePhase.BATTLE_START:
                battleOver = false;
                await Task.Delay(250);
                await sendOutPokemon();
                changeBattlePhase(BattlePhase.PLAYER_PLANNING);
                break;
            case BattlePhase.PLAYER_PLANNING:
                Debug.Log("Player Planning Phase");
                //Get all player battle actions
                List<BattleAction> playerBattleActions = await BattleMenu.SelectActions();
                //Add them all to their respective queue
                foreach (BattleAction action in playerBattleActions)
                {
                    addBattleActionToQueue(action);
                }
                changeBattlePhase(BattlePhase.ENEMY_PLANNING);
                break;
            case BattlePhase.ENEMY_PLANNING:
                Debug.Log("Enemy Planning Phase");
                changeBattlePhase(BattlePhase.BATTLE);
                break;
            case BattlePhase.BATTLE:
                Debug.Log("Battling Phase...");
                await readBattleActionsScript();
                printBattleActions();
                break;
        }
    }

    public async Task sendOutPokemon()
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

            script += randomPokemon.displayName;
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

            script += playerPokemon[i].displayName;
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

        battleBuffer[bufferEnum].Add(battleAction);
    }

    private void printBattleActions()
    {
        foreach (KeyValuePair<BattleBuffer, List<BattleAction>> entry in battleBuffer)
        {
            Debug.Log(entry.Key);
            foreach (BattleAction action in entry.Value)
            {
                Debug.Log(action.ToString());
            }
        }
    }

    private async Task readBattleActionsScript()
    {
        foreach (KeyValuePair<BattleBuffer, List<BattleAction>> entry in battleBuffer)
        {
            foreach (BattleAction action in entry.Value)
            {
                await BattleMessage.performScript(action.script());
            }
        }
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

    private static async void TwoClock()
    {
        while (true)
        {
            await Task.Delay(250);
            BattleController.tick++;
            BattleController.tick %= 2;
        }
    }
}
