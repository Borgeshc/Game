using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    string saveData;
    Controls controls;
    Movement movement;
    Weapon currentWeapon;

    void Start ()
    {
        movement = GetComponent<Movement>();
	}
    
    void Update ()
    {
        Move(controls.Move.Y, controls.Move.X);
	}

    void Move(float _vertical, float _horizontal)
    {
        movement.Move(_vertical, _horizontal);
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
