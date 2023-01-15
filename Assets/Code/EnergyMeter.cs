using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnergyMeter : MonoBehaviour
{
    [SerializeField]
    private PlayerController _Player;
    [SerializeField]
    private TextMeshProUGUI _Energy;

    private float _CurrentEnegy;

    public void Update()
    {
        /*
        if(_CurrentEnegy != _Player.Energy)
        {
            _CurrentEnegy = _Player.Energy;
            _Energy.SetText($"Energy: {_Player.Energy.ToString("P")}");
        }
        */
    }
}
