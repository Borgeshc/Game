using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static bool canMove;
    public static bool isDead;
    public static GameObject target;

    public LayerMask targetableLayer;

    public static LayerMask _targetableLayer;

    static GameObject player;

    string saveData;
    Controls controls;
    Movement movement;
    Attack attack;
    Weapon currentWeapon;
    CameraFollow myCamera;
    CameraShake cameraShake;

    void Start ()
    {
        movement = GetComponent<Movement>();
        attack = GetComponent<Attack>();
        myCamera = Camera.main.GetComponent<CameraFollow>();
        cameraShake = myCamera.GetComponent<CameraShake>();
        canMove = true;
        player = gameObject;

        _targetableLayer = targetableLayer;
	}
    
    void Update ()
    {
        if(target != null)

        RaycastHit hit;
        if (Physics.CapsuleCast(player.transform.position, player.transform.forward, 5, player.transform.forward, out hit, 20, _targetableLayer))
            target = hit.transform.gameObject;

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

    void Move(float vertical, float horizontal)
    {
        movement.Move(vertical, horizontal);
    }

    void Look(float horizontal)
    {
        if(!cameraShake.Shaking)
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
