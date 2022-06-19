using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sits on GameManager object.
/// Handles the player energy
/// </summary>

public class EnergyHandler : MonoBehaviour
{
    public static EnergyHandler Instance;
    public Text energyText;

    void Awake()
    {
        Instance = this;
    }

    public void ResetEnergy()
    {
        GameManager.Instance.playerEnergy = GameManager.Instance.playerMaxEnergy;
        SetEnergyText();
    }

    public bool HaveEnoughEnergy(int cost)
    {
        if (cost > GameManager.Instance.playerEnergy) return false;

        GameManager.Instance.playerEnergy -= cost;
        SetEnergyText();
        return true;
    }

    public void SetEnergyText()
    {
        energyText.text = GameManager.Instance.playerEnergy.ToString() + "/" + GameManager.Instance.playerMaxEnergy.ToString();
    }
}
