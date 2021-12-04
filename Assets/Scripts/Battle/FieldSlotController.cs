using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/* This class performs all battleActions taken against a pokemon.
 * The pokemon class holds the data for the pokemon, the fieldslot
 * takes damage, faints, is targetted by abilities, has a healthbar etc.
 * */
public class FieldSlotController : MonoBehaviour
{
    public Pokemon pokemon;
    private Animator pokeball;
    private SpriteRenderer pokeSprite;
    public HealthbarController healthbar;
    [SerializeField] private BattleController battleController;

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

    public void setPokemon(Pokemon pokemonData, bool front = true)
    {
        pokemon = Pokemon.copy(pokemonData);
        pokemon.fieldSlot = this;
        if (pokemon == null) return;

        healthbar.gameObject.SetActive(true);
        healthbar.setPokemon(pokemon);
        string path = "Sprites/Pokemon/pokemon" + (front ? "" : "Backs");
        pokeSprite.sprite = Resources.LoadAll<Sprite>(path)[(int)(pokemon.name)];
    }

    public void pokemonPlaySendIn()
    {
        pokeSprite.GetComponent<Animator>().SetTrigger("SendIn");
    }

    /* This damage has already been calculated through the BattleMove itself.
     * The fieldslot controls damaging the pokemon, while the healthbarController deals
     * with animating the slider.
     * */
    public async Task takeDamage(int amount)
    {
        //Damage pokemon and ensure it does not overdamage or overheal
        pokemon.current_hp -= amount;
        int maxHp = pokemon.getStatValue(Stats.HP);
        pokemon.current_hp = Mathf.Clamp(pokemon.current_hp, 0, maxHp);

        //Wait for slider to animate to new value
        float targetSliderValue = (float)pokemon.current_hp / maxHp;
        await healthbar.SetTargetValue(targetSliderValue);

        if (pokemon.current_hp <= 0)
        {
            await FaintPokemon();
        }

        await Task.Delay(100);
    }

    /* The fieldslot controller is responsible for making pokemon faint actively in battle
     * We need to remove the pokemon's actions from the BattleQueue, play a "has fainted"
     * message, and play a fainting animation/cry
     * */
    public async Task FaintPokemon()
    {
        battleController.removeFromQueue(pokemon);
        await battleController.BattleMessage.performScript(pokemon.displayName + " fainted!");
        await Task.Delay(250);
        //TODO: pokemon faint animation
        await Task.Delay(250);
        pokeSprite.enabled = false;
    }

    /* This is how we can check that a fieldslot contains a battle-ready pokemon
     * */
    public bool isAvailable()
    {
        return (pokemon != null) && !pokemon.isFainted();
    }
}
