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

    [SerializeField] private Text nameText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text levelText;
    [SerializeField] private Image statusBadge;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void setPokemon(Pokemon pokemon)
    {
        this.pokemon = pokemon;
        setHealthText();
        levelText.text = "Lv: " + pokemon.level;
        nameText.text = pokemon.displayName;
    }

    public async Task SetTargetValue(float targetSliderValue)
    {
        while (healthBar.value - 0.01f > targetSliderValue)
        {
            healthBar.value = Mathf.SmoothDamp(healthBar.value, targetSliderValue,
                ref currentSliderVelocity, SMOOTH_TIME, MAX_SPEED);
            setHealthText();
            await Task.Yield();
        }
        healthBar.value = targetSliderValue;
        setHealthText();
    }

    private void setHealthText()
    {
        if (pokemon == null) return;
        if (healthText != null)
        {
            int maxHp = pokemon.getStatValue(Stats.HP);
            healthText.text = Mathf.Round(maxHp * healthBar.value) + " / " + maxHp;
        }
    }

    public void setStatusBadge(Sprite badgeSprite)
    {
        statusBadge.enabled = (badgeSprite != null);
        statusBadge.sprite = badgeSprite;
    }
}
