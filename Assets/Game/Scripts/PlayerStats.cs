using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Tooltip("Converts to Health at a 1 to 10 Ratio.")]
    public int _vitality;
    [Tooltip("Your chance to crit with an attack. 1 to 1 Ratio")]
    public int _critChance;

    public static int vitality;
    public static int critChance;

	void Start ()
    {
        vitality = _vitality;
        critChance = _critChance;
	}
}
