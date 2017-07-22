using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth;
    public Image healthBar;

    float health;
    Animator anim;

	void Start ()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        UpdateHealthBar();
	}

    public void TookDamage(float damage)
    {
        Hit();
        health -= damage;
        UpdateHealthBar();

        if(health <= 0)
        {
            Died();
        }
    }

    void Hit()
    {
        anim.SetTrigger("Hit");
    }

	void UpdateHealthBar ()
    {
        healthBar.fillAmount = health / maxHealth;
	}

    void Died()
    {
        anim.SetBool("Died", true);
    }
}
