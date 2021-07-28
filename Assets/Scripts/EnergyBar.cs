using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public Slider slider;
    //public TextMeshPro energyNumber;

    public void SetMaxEnergy(int energy)
    {
        slider.maxValue = energy;
        slider.value = energy;
        //energyNumber.text = energy.ToString();
    }

    public void SetCurrentEnergy(int energy)
    {
        slider.value = energy;
        //energyNumber.text = energy.ToString();
    }

    public void IndicateEnergyUsage(int amountUsed)
    {

    }
}
