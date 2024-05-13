using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private BossEnemy enemy;

    private void OnEnable()
    {

        BossEnemy.OnTakeDamage += UpdateHealthBar;
    }
    private void OnDisable()
    {
        BossEnemy.OnTakeDamage -= UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = enemy.currentHealth / (float)enemy.Health;
        if(healthBar.fillAmount <= 0 )
            gameObject.SetActive(false);
    }
}
