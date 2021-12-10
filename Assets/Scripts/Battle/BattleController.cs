using Febucci.UI;
using HelperFunctions;
using StatusEffects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public AudioSource audio;
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
    private Dictionary<List<BattleAction>, BattleAction> removalQueue;
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
        for (int i = 0; i < EnumHelper.GetLength<BattleBuffer>(); i++)
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
                cam.Reset();
                await Task.Delay(500);
                await endOfTurnStatusEffects();
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
            if (!fc.isAvailable()) continue;
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
            List<FieldSlotController> availableEnemies = enemyFieldSlots.FindAll(e => e.isAvailable());
            List<FieldSlotController> availableAllies = allyFieldSlots.FindAll(a => a.isAvailable());

            switch (move.targets) {
                case Targets.ALL:
                    targets.AddRange(availableAllies.Concat(availableEnemies).ToList());
                    break;
                case Targets.ALLIES:
                    targets.AddRange(availableEnemies);
                    break;
                case Targets.ALLY:
                    targets.Add(enemyFieldSlots[Random.Range(0, enemyFieldSlots.Count)]);
                    break;
                case Targets.ALL_BUT_SELF:
                    targets.AddRange(availableAllies.Concat(availableEnemies).ToList());
                    targets.Remove(move.getPokemon().fieldSlot);
                    break;
                case Targets.ENEMIES:
                    targets.AddRange(availableAllies);
                    break;
                case Targets.ENEMY:
                    targets.Add(allyFieldSlots[Random.Range(0, allyFieldSlots.Count)]);
                    break;
                case Targets.SELF:
                    targets.Add(move.getPokemon().fieldSlot);
                    break;
                default:
                    throw new System.Exception("No Enemy target handler for Targets enum: " + move.targets.ToString());
            }

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

    /* Right now we just use this for fainting. If a Pokemon faints we should
     * remove all of their queued actions
     * */
    public void removeFromQueue(Pokemon pokemon)
    {
        //Search through every queue in the bufferr
        foreach (var queue in battleBuffer)
        {
            //Access the List of BattleActions
            List<BattleAction> battleActions = queue.Value;
            foreach (var action in battleActions)
            {
                /* Remove all BattleActions from the queue whose source is 
                * the pokemon in question
                * */
                if (action.source == pokemon)
                {
                    //clearQueues() will delete the action
                    //this bool prevents it from firing.
                    action.toBeRemoved = true;
                }
            }
        }
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
                if (action.toBeRemoved) continue;
                if (action.source.isFainted()) continue;

                if (action.GetType().Equals(typeof(BattleMove)))
                {
                    BattleMove move = (BattleMove)action;

                    #region checkStatus
                    move.getPokemon().clearStatuses();
                    move.getPokemon().statuses.Sort();
                    List<PokemonStatus> statuses = move.getPokemon().statuses.FindAll(s => s.getBuffer().Equals(BattleBuffer.BATTLE_MOVE));
                    foreach (PokemonStatus status in statuses)
                    {
                        BattleMove newMove = await status.DoInterruptStatus(BattleMessage);
                        if (newMove != null)
                        {
                            if (newMove.getMove().GetType().Equals(typeof(SkipMove)))
                            {
                                //Paralysis, Sleep
                                goto SkipMove;
                            }
                            //Confusion HitSelf or Uncontrollable Move
                            move = newMove;
                            break;
                        }
                        //Confusion or paralysis did not work
                        //Sleep wake up?
                    }
                    #endregion

                    #region FixTargets
                    //Fix targets
                    List<FieldSlotController> fixedTargets = new List<FieldSlotController>();
                    //For each intended target...
                    foreach (FieldSlotController target in move.targets)
                    {
                        //If it is available we keep it (add to list of fixedTargets)
                        if (target.isAvailable())
                        {
                            fixedTargets.Add(target);
                            continue;
                        }
                        else
                        //If the target is not available (e.g. fainted)
                        {
                            //Then if this is an AoE ability, exclude the target
                            if (move.targets.Count > 1)
                            {
                                continue;
                            }
                            else
                            //But if it was a single target we must reroute it
                            {
                                //So we get all fieldControllers of either enemies or allies
                                List<FieldSlotController> availableTargets =
                                    target.isEnemy ? enemyFieldSlots : allyFieldSlots;
                                //Filter out all of the ones which are available
                                availableTargets = availableTargets.FindAll(t => t.isAvailable());
                                //And pick a random new target from that list
                                FieldSlotController nextTarget = availableTargets[0]; //availableTargets[Random.Range(0, availableTargets.Count - 1)];
                                //Then we add it to the fixed list
                                fixedTargets.Add(nextTarget);
                            }
                        }
                    }
                    move.targets = fixedTargets;
                    //We need to recalculate the damage the move deals to the new target
                    move.setTargetDamages();
                    #endregion

                    List<string> script = move.script();
                    await BattleMessage.performScript(script[0]);

                    AudioClip clip = Resources.Load<AudioClip>("Sounds/BattleMoves/" + move.getMove().getName());
                    if (clip == null)
                    {
                        clip = Resources.Load<AudioClip>("Sounds/BattleMoves/Tackle");
                    }
                    audio.PlayOneShot(clip);

                    foreach (FieldSlotController fc in move.getTargets())
                    {
                        script.RemoveAt(0);
                        await BattleMessage.performScript(script[0]);

                        AudioClip hitSound = Resources.Load<AudioClip>("Sounds/BattleMoves/Hit Normal Damage");
                        audio.PlayOneShot(hitSound);
                        await fc.takeDamage(move.getDamage(fc));
                    }

                SkipMove:
                    continue;
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


    private async Task endOfTurnStatusEffects()
    {
        List<FieldSlotController> allFcs = new List<FieldSlotController>();
        allFcs.AddRange(allyFieldSlots);
        allFcs.AddRange(enemyFieldSlots);
        allFcs = allFcs.FindAll(f => f.isAvailable());
        foreach (FieldSlotController fc in allFcs)
        {
            Pokemon pokemon = fc.pokemon;
            List<PokemonStatus> statuses = pokemon.statuses.FindAll(s => s.getBuffer().Equals(BattleBuffer.TURN_END));
            foreach (PokemonStatus status in statuses)
            {
                await status.DoStatus(BattleMessage);
            }
        }
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
