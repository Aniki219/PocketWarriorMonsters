using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSlotController : MonoBehaviour
{
    public Pokemon pokemon;
    private SpriteRenderer pokeSprite;

    // Start is called before the first frame update
    void Start()
    {
        pokeSprite = transform.Find("PokemonSprite")
            .GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playBallAnimation()
    {

    }

    public void setPokemon(Pokemon pokemonData, bool front = true)
    {
        pokemon = Pokemon.copy(pokemonData);
        string path = "Sprites/Pokemon/pokemon" + (front ? "" : "Backs");
        pokeSprite.sprite = Resources.LoadAll<Sprite>(path)[(int)(pokemon.name)];
    }
}
