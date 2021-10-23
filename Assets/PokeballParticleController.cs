using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeballParticleController : MonoBehaviour
{
    public enum PokeballType {
        CHERISH,
        DIVE,
        DREAM,
        DUSK,
        GREAT,
        HEAL,
        LUXURY,
        MASTER,
        NEST,
        NET,
        POKE,
        PREMIER,
        QUICK,
        SAFARI,
        SPORT,
        TIMER,
        ULTRA
    }

    public PokeballType pokeballType = PokeballType.POKE;
    public PokemonName spawnPokemon = PokemonName.BULBASAUR;

    public FieldSlotController fieldSlot;

    // Start is called before the first frame update
    void Start()
    {
        fieldSlot = GetComponentInParent<FieldSlotController>();
        GetComponentInChildren<Animator>().SetFloat("BallNumber", (int)pokeballType);
    }

    public void createSpawnPokemon()
    {
        fieldSlot.pokemonPlaySendIn();
    }
}
