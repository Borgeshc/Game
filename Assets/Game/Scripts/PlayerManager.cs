using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static bool canMove;
    public static bool isDead;

    static GameObject player;

    string saveData;
    Controls controls;
    Movement movement;
    Attack attack;
    Weapon currentWeapon;
    CameraFollow myCamera;

    void Start ()
    {
        movement = GetComponent<Movement>();
        attack = GetComponent<Attack>();
        myCamera = Camera.main.GetComponent<CameraFollow>();
        canMove = true;
        player = gameObject;
	}
    
    void Update ()
    {
        if (canMove)
        {
            Move(controls.Move.Y, controls.Move.X);
        }
        if(controls.Fire)
            PrimaryFire();

        if (controls.Aim)
            SecondaryFire();
    }

    private void LateUpdate()
    {
        Look(controls.Look.X);
    }

    public static GameObject TargetingSystem()
    {
        RaycastHit hit;
        if (Physics.CapsuleCast(player.transform.position, player.transform.forward * 20, 5, player.transform.forward, out hit))
            return hit.transform.gameObject;
        else return null;
    }

    void Move(float vertical, float horizontal)
    {
        movement.Move(vertical, horizontal);
    }

    void Look(float horizontal)
    {
        myCamera.Look(horizontal);
    }

    void PrimaryFire()
    {
        attack.PrimaryFire();
    }

    void SecondaryFire()
    {
        attack.SecondaryFire();
    }

    public void SetWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
        movement.UpdateAnimator(currentWeapon);
    }

    public Weapon GetWeapon()
    {
        return currentWeapon;
    }

    #region InControl
    void OnEnable()
    {
        controls = Controls.CreateWithDefaultBindings();
    }

    void OnDisable()
    {
        controls.Destroy();
    }

    void SaveBindings()
    {
        saveData = controls.Save();
        PlayerPrefs.SetString("Bindings", saveData);
    }

    void LoadBindings()
    {
        if (PlayerPrefs.HasKey("Bindings"))
        {
            saveData = PlayerPrefs.GetString("Bindings");
            controls.Load(saveData);
        }
    }
    #endregion
}
