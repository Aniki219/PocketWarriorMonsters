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
    public BattlePlanController battlePlan;

    public static CameraController cam;
    public Canvas canvas;

    private bool battleOver;

    //This is for icon walking animations. Keeps them in sync and simplifies code.
    public static int tick = 0;
    public static int currentPokemonIndex = 0;
    public static int currentTargetIndex = 0;

    public enum BattlePhase
    {
        BATTLE_START,
        PLAYER_PLANNING,
        ENEMY_PLANNING,
        BATTLE,
        TURN_END
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
        /* Setup  Cameras:
         * The UICamera needs the Canvas too be set to Overlay
         * but doing so makes it impossible to work with the
         * canvas in the editor.
         * So we  set it to ScreenSpace by default, and then
         * when the battle starts we switch it to Overlay and
         * remove the UI layer from our cullingMask.
         * */
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        cam = Camera.main.GetComponent<CameraController>();
        Camera.main.cullingMask = ~LayerMask.GetMask("UI");
        //Start our async clock
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
                cam.Reset();
                await Task.Delay(1000);
                
                List<BattleAction> enemyBattleActions = getEnemyBattleActions();
                //Add them all to their respective queue
                foreach (BattleAction action in enemyBattleActions)
                {
                    addBattleActionToQueue(action);
                }
                changeBattlePhase(BattlePhase.BATTLE);
                break;

            case BattlePhase.BATTLE:
                Debug.Log("Battling Phase...");
                sortBattleBuffers();
                await readBattleActionsScript();
                changeBattlePhase(BattlePhase.TURN_END);
                break;

            case BattlePhase.TURN_END:
                //printBattleActions();
                await Task.Delay(1000);
                battlePlan.Clear();
                clearQueues();
                changeBattlePhase(BattlePhase.PLAYER_PLANNING);
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

    /* During Enemy Battle Phase we select random moves and random targets.
     * Eventually we will consider random items and pokemon swaps.
     * */
    private List<BattleAction> getEnemyBattleActions()
    {
        List<BattleAction> enemyBattleActions = new List<BattleAction>();

        foreach (FieldSlotController fc in enemyFieldSlots)
        {
            Pokemon pokemon = fc.pokemon;

            /* We get a list of all moves with PP.
             * If no moves have PP the pokemon should use struggle.
             * We dont have struggle yet soo we'll use tackle and log a warning
             * to pester ourselves.
             * */
            List<PokemonMove> moves = pokemon.moves.FindAll(m => m.getCurrentPp() > 0);
            PokemonMove move;

            if (moves.Count > 0) {
                int i = Random.Range(0, moves.Count);
                move = moves[i];
            } else
            {
                /*Hey man, if you made it far enough to implement struggle for real
                * you're a fucking boss okay? Like damn who gives a shit right now.
                * 11/27/21
                * */
                Debug.LogWarning(pokemon.displayName + "is using struggle!");
                move = new StandardMove(Moves.TACKLE, pokemon);
            }

            /* Selecting a target will need to eventually check if it is a valid target
             * During the battle  a target may become invalid but we will handle it
             * in the doBattle method
             * */
            List<FieldSlotController> targets = new List<FieldSlotController>();
            targets.Add(allyFieldSlots[Random.Range(0, 2)]);

            move.decPp();
            enemyBattleActions.Add(new BattleMove(move, targets));
        }
        return enemyBattleActions;
    }

    public void addBattleActionToQueue(BattleAction battleAction, BattleBuffer? overrideBuffer = null)
    {
        /*We can automatically assign the correct buffer by asking the BattleAction which buffer
         * it belongs to. I think this is sufficient but just incase we have an overrideBuffer
         * parameter which can sort out ambiguities. Meaning if we want to explicitly put a
         * BattleAction into a buffer that it woouldn't default into we can override that 
         * destination. Maybe an Item will be added to the BattleMove buffer or something
         */
        BattleBuffer bufferEnum = (overrideBuffer.HasValue) ? overrideBuffer.Value : 
            battleAction.getBattleBuffer();

        battleBuffer[bufferEnum].Add(battleAction);
    }

    private void sortBattleBuffers()
    {
        battleBuffer[BattleBuffer.BATTLE_MOVE].Sort();
    }

    private void printBattleActions()
    {
        foreach (KeyValuePair<BattleBuffer, List<BattleAction>> entry in battleBuffer)
        {
            Debug.Log(entry.Key + " Buffer");
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
                if (action.GetType().Equals(typeof(BattleMove)))
                {
                    BattleMove move = (BattleMove)action;
                    foreach (FieldSlotController fc in move.getTargets())
                    {
                        await fc.takeDamage(move.getDamage(fc));
                    }
                }
                cam.Reset();
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

    /* We just need a clock that ticks back and forth for a lot of 
     * pokemon icon animations. Figure we'll just have the BattleController
     * handle that to simplify code and keep all in sync.
     * */
    private static async void TwoClock()
    {
        while (true)
        {
            await Task.Delay(250);
            tick++;
            tick %= 2;
        }
    }
}
