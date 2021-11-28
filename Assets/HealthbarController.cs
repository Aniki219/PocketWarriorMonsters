using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour
{
    public FieldSlotController fieldSlot;
    private Pokemon pokemon;
    [SerializeField] private Slider healthBar;

    private const float SMOOTH_TIME = 0.250f;
    private const float MAX_SPEED = 10;
    private float currentSliderVelocity;

    private void Start()
    {
        setPokemon(fieldSlot.pokemon);
        if (pokemon == null)
        {
            gameObject.SetActive(false);
        }
    }

    public void setPokemon(Pokemon pokemon)
    {
        this.pokemon = pokemon;
    }

    /* This damage has already been calculated through the BattleMove itself
     * */
    public async Task takeDamage(int amount)
    {
        int maxHp = pokemon.getStatValue(Stats.HP);
        //The Pokemon should really be incharge of this
        pokemon.current_hp -= amount;
        pokemon.current_hp = Mathf.Clamp(pokemon.current_hp, 0, maxHp);
        float targetSliderValue = (float)pokemon.current_hp / maxHp;

        while(healthBar.value - 0.01f > targetSliderValue)
        {
            healthBar.value = Mathf.SmoothDamp(healthBar.value, targetSliderValue, 
                ref currentSliderVelocity, SMOOTH_TIME, MAX_SPEED);
            await Task.Yield();
        }
        healthBar.value = targetSliderValue;

        await Task.Delay(100);
    }
}
