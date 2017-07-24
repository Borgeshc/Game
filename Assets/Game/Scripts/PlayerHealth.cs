using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image healthBar;
    public AudioSource source;
    public AudioClip[] hurtSounds;

    Animator anim;

    float maxHealth;
    float health;

    void Start()
    {
        maxHealth = (float)PlayerStats.vitality * 10;
        health = maxHealth;
        anim = GetComponent<Animator>();
        UpdateHealthBar();
    }

    public void UpdateMaxHealth()
    {
        maxHealth = (float)PlayerStats.vitality * 10;
        UpdateHealthBar();
    }

    public void TookDamage(float damage, int critChance)
    {
        if (PlayerManager.isDead) return;

        Hit();

        if (CritChance(critChance))
        {
            health -= damage * 2;
        }
        else
        {
            health -= damage;
        }
        print(health);
        UpdateHealthBar();

        if (health <= 0)
        {
            Died();
        }
    }

    bool CritChance(int critChance)
    {
        int critRoll = Random.Range(0, 100);
        if (critRoll <= critChance)
            return true;
        else
            return false;
    }

    void Hit()
    {
        anim.SetTrigger("Hit");
        source.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = health / maxHealth;
    }

    void Died()
    {
        PlayerManager.isDead = true;
        anim.SetBool("Died", true);

        Destroy(gameObject, 10);
    }

    public void DeathSound(AudioClip deathSound)
    {
        source.PlayOneShot(deathSound);
    }
}
