using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth;
    public Image healthBar;
    public Text regularCombatText;
    public Text criticalCombatText;

    EnemyAI enemyAI;
    float health;
    Animator anim;

    bool isDead;

	void Start ()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        UpdateHealthBar();
	}

    public void TookDamage(float damage)
    {
        if (isDead) return;
        Hit();

        if (CritChance())
        {
            StartCoroutine(FloatingCombatText((damage * 2), criticalCombatText));
            health -= damage * 2;
        }
        else
        {
            StartCoroutine(FloatingCombatText(damage, regularCombatText));
            health -= damage;
        }

        UpdateHealthBar();

        if(health <= 0)
        {
            Died();
        }
    }

    bool CritChance()
    {
        int critRoll = Random.Range(0, 100);
        if (critRoll <= PlayerStats.critChance)
            return true;
        else
            return false;
    }

    void Hit()
    {
        anim.SetTrigger("Hit");
    }

    IEnumerator FloatingCombatText(float damagedAmt, Text combatText)
    {
        yield return new WaitForSeconds(.2f);
        combatText.gameObject.SetActive(true);
        combatText.text = damagedAmt.ToString();

        yield return new WaitForSeconds(.2f);
        criticalCombatText.gameObject.SetActive(false);
        regularCombatText.gameObject.SetActive(false);
    }

	void UpdateHealthBar ()
    {
        healthBar.fillAmount = health / maxHealth;
	}

    void Died()
    {
        isDead = true;
        enemyAI.Died();
        GetComponent<Collider>().enabled = false;
        anim.SetBool("Died", true);

        Destroy(gameObject, 10);
    }
}
