using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Animator anim;
    public AudioSource source;

    public int minPrimaryDamage;
    public int maxPrimaryDamage;

    public int minSecondaryDamage;
    public int maxSecondaryDamage;

    public AudioClip leftWeaponClip;
    public AudioClip rightWeaponClip;
    public GameObject leftMuzzleFlash;
    public GameObject rightMuzzleFlash;

    bool onGlobalCooldown;
    int primaryFire;
    int secondaryFire;

    public void PrimaryFire()
    { 
        if(!onGlobalCooldown)
        {
            PlayerManager.canMove = false;
            anim.SetBool("PrimaryFiring", true);
            primaryFire++;

            if (primaryFire > 3)
                primaryFire = 1;

            anim.SetInteger("PrimaryFire", primaryFire);

            if (PlayerManager.TargetingSystem().tag.Equals("Enemy"))
            {
                PlayerManager.TargetingSystem().GetComponent<EnemyHealth>().TookDamage(Random.Range(minPrimaryDamage, maxPrimaryDamage));
            }

            onGlobalCooldown = true;
            StartCoroutine(GlobalCooldown());
        }
    }

    public void PrimaryWeaponFired()
    {
        source.clip = leftWeaponClip;
        source.Play();

     

        StartCoroutine(MuzzleFlash(leftMuzzleFlash));
    }

    public void SecondaryFire()
    {
        if (!onGlobalCooldown)
        {
            PlayerManager.canMove = false;
            anim.SetBool("SecondaryFiring", true);
            secondaryFire++;

            if (secondaryFire > 3)
                secondaryFire = 1;

            anim.SetInteger("SecondaryFire", secondaryFire);

            if (PlayerManager.TargetingSystem().tag.Equals("Enemy"))
            {
                PlayerManager.TargetingSystem().GetComponent<EnemyHealth>().TookDamage(Random.Range(minSecondaryDamage, maxSecondaryDamage));
            }

            onGlobalCooldown = true;
            StartCoroutine(GlobalCooldown());
        }
    }

    public void SecondaryWeaponFired()
    {
        source.clip = rightWeaponClip;
        source.Play();

        StartCoroutine(MuzzleFlash(rightMuzzleFlash));
    }

    IEnumerator MuzzleFlash(GameObject muzzleflash)
    {
        muzzleflash.SetActive(true);
        yield return new WaitForSeconds(.2f);
        muzzleflash.SetActive(false);
    }


    IEnumerator GlobalCooldown()
    {
        yield return new WaitForSeconds(.5f);
        onGlobalCooldown = false;
        PlayerManager.canMove = true;
        anim.SetBool("PrimaryFiring", false);
        anim.SetBool("SecondaryFiring", false);
    }

    public void Ability1()
    {

    }
}
