using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Movement : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public RuntimeAnimatorController[] weaponAnimators;

    public static bool canMove;

    Dictionary<string, RuntimeAnimatorController> findWeaponAnimations = new Dictionary<string, RuntimeAnimatorController>();

    Vector3 movement;
    Animator animator;
    AnimatorOverrideController animatorOverrideController;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        canMove = true;

        foreach(RuntimeAnimatorController weaponAnimator in weaponAnimators)
        {
            findWeaponAnimations.Add(weaponAnimator.name, weaponAnimator);
        }
    }

    public void Move(float vertical, float horizontal)
    {
        if (canMove)
        {
            Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;
            Vector3 right = new Vector3(forward.z, 0, -forward.x);
            movement = ((horizontal * right) + (vertical * forward)) * Time.deltaTime;


            if (movement != Vector3.zero)
            {
                animator.SetBool("IsIdle", false);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * rotationSpeed);
                transform.position += transform.forward * speed * Time.deltaTime;
            }
            else
                animator.SetBool("IsIdle", true);

            MovementAnimations(vertical, horizontal);
        }
    }

    public void UpdateAnimator(Weapon newWeapon)
    {
        animatorOverrideController = new AnimatorOverrideController(findWeaponAnimations[newWeapon.name]);
        animator.runtimeAnimatorController = animatorOverrideController;
    }

    void MovementAnimations(float vertical, float horizontal)
    {
        animator.SetFloat("Vertical", vertical);
        animator.SetFloat("Horizontal", horizontal);
    }
}
