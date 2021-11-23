using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSlotController : MonoBehaviour
{
    public Pokemon pokemon;
    private Animator pokeball;
    private SpriteRenderer pokeSprite;

    public bool isEnemy = false;

    // Start is called before the first frame update
    void Start()
    {
        pokeSprite = transform.Find("PokemonSprite")
            .GetComponent<SpriteRenderer>();
        pokeball = transform.Find("ThrownPokeballParticle")
            .GetComponentInChildren<Animator>();
    }

    public void playBallAnimation()
    {
        pokeball.SetFloat("BallNumber", Random.Range(0, 17));
        pokeball.SetTrigger("Throw");
    }

    public void setPokemon(Pokemon pokemonData, bool front = true)
    {
        pokemon = Pokemon.copy(pokemonData);
        string path = "Sprites/Pokemon/pokemon" + (front ? "" : "Backs");
        pokeSprite.sprite = Resources.LoadAll<Sprite>(path)[(int)(pokemon.name)];
    }

    public void pokemonPlaySendIn()
    {
        pokeSprite.GetComponent<Animator>().SetTrigger("SendIn");
    }
}
