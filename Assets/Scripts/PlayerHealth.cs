using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    private float currentHealth;

    [Header("UI")]
    public Slider healthSlider;

    private void Start()
    {
        healthSlider.maxValue = health;
        currentHealth = health;
    }

    private void Update()
    {
        healthSlider.value = currentHealth;
    }

    public void DecreaseHealth(float decreaseAmount)
    {
        currentHealth -= decreaseAmount;

        if (currentHealth < 0)
            currentHealth = 0;

        Debug.Log("Health decrease");
    }

    public void IncreaseHealth(float increaseAmount)
    {
        currentHealth += increaseAmount;

        if (currentHealth > health)
            currentHealth = health;

        Debug.Log("Health increase");
    }
}
