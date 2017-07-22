using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Animator anim;
    public AudioSource source;

    bool onGlobalCooldown;
    int primaryFire;
    int secondaryFire;

    public void PrimaryFire()
    { 
        if(!onGlobalCooldown)
        {
            Movement.canMove = false;
            anim.SetBool("PrimaryFiring", true);
            primaryFire++;

            if (primaryFire > 3)
                primaryFire = 1;

            anim.SetInteger("PrimaryFire", primaryFire);

            onGlobalCooldown = true;
            StartCoroutine(GlobalCooldown());
        }
    }

    public void PlaySound(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    public void SecondaryFire()
    {
        if (!onGlobalCooldown)
        {
            Movement.canMove = false;
            anim.SetBool("SecondaryFiring", true);
            secondaryFire++;

            if (secondaryFire > 3)
                secondaryFire = 1;

            anim.SetInteger("SecondaryFire", secondaryFire);

            onGlobalCooldown = true;
            StartCoroutine(GlobalCooldown());
        }
    }

    IEnumerator GlobalCooldown()
    {
        yield return new WaitForSeconds(.5f);
        onGlobalCooldown = false;
        Movement.canMove = true;
        anim.SetBool("PrimaryFiring", false);
        anim.SetBool("SecondaryFiring", false);
    }

    public void Ability1()
    {

    }
}
