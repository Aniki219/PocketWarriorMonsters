using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleController : MonoBehaviour
{
    public List<BattleAction> startBattleBuffer;
    public List<BattleAction> startTurnBuffer;
    public List<BattleAction> swapPokemonBuffer;
    public List<BattleAction> itemBuffer;
    public List<BattleAction> priorityMoveBuffer;
    public List<BattleAction> battleMoveBuffer;
    public List<BattleAction> turnEndBuffer;

    public FieldSlotController[] allyFieldSlots = new FieldSlotController[3];
    public FieldSlotController[] enemyFieldSlots = new FieldSlotController[3];

    public GameObject BattleMessage;

    public List<KeyValuePair<GameObject, UnityAction>> buttonActions;

    private BattlePhase battlePhase = BattlePhase.PLAYER_PLANNING;

    public enum BattlePhase
    {
        PLAYER_PLANNING,
        ENEMY_PLANNING,
        BATTLE
    }
    // Start is called before the first frame update
    void Start()
    {
        startBattleBuffer = new List<BattleAction>();
        startTurnBuffer = new List<BattleAction>();
        swapPokemonBuffer = new List<BattleAction>();
        itemBuffer = new List<BattleAction>();
        priorityMoveBuffer = new List<BattleAction>();
        battleMoveBuffer = new List<BattleAction>();
        turnEndBuffer = new List<BattleAction>();

        sendOutPokemon();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.getKeyPressed("Space"))
        {
            BattleMessage.GetComponent<Animator>().SetTrigger("Next");
        }
    }

    void sendOutPokemon()
    {
        MoveData move = MoveDataReader.getMoveData(Moves.AERIAL_ACE);
        Debug.Log(move.name);
        //Player Pokemon
        List<Pokemon> pokemonList = PlayerPokemon.instance.pokemon;
        int num = pokemonList.Count;
        for (int i = 0; i < num && i < 3; i++)
        {
            allyFieldSlots[i].setPokemon(pokemonList[i], false);
            allyFieldSlots[i].playBallAnimation();
        }
    }
}
