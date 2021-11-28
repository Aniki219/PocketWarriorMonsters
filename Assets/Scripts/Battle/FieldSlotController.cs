using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FieldSlotController : MonoBehaviour
{
    public Pokemon pokemon;
    private Animator pokeball;
    private SpriteRenderer pokeSprite;
    public HealthbarController healthbar;

    public bool isEnemy = false;
    public int slotNumber;

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

    public async Task takeDamage(int damage)
    {
        await healthbar.takeDamage(damage);
    }

    public void setPokemon(Pokemon pokemonData, bool front = true)
    {
        pokemon = Pokemon.copy(pokemonData);
        //We should change this too set a healthbar reference in the pokemon
        healthbar.setPokemon(pokemon);
        string path = "Sprites/Pokemon/pokemon" + (front ? "" : "Backs");
        pokeSprite.sprite = Resources.LoadAll<Sprite>(path)[(int)(pokemon.name)];
    }

    public void pokemonPlaySendIn()
    {
        pokeSprite.GetComponent<Animator>().SetTrigger("SendIn");
    }
}
