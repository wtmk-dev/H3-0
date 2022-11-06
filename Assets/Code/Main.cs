using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private CharacterCreator _CharacterCreator;
    [SerializeField]
    private List<CharacterCreationView> _CC_Views;
    [SerializeField]
    private List<CharacterSkin> _Players;
    [SerializeField]
    private Canvas _Canvas;

    private bool _HasGameStart, _WillRespawn;
    private List<PlayerController> _Inputs = new List<PlayerController>();

    private void Start()
    {
        _CharacterCreator = new CharacterCreator(_CC_Views);
        _CharacterCreator.StartGame += OnStartGame;

        for (int i = 0; i < _Players.Count; i++)
        {
            _Inputs.Add(_Players[i].GetComponent<PlayerController>());
        }

        OnStartGame();
    }

    private void OnStartGame()
    {
        _HasGameStart = true;
        _Canvas.gameObject.SetActive(false);

        for (int i = 0; i < _Inputs.Count; i++)
        {
            _Inputs[i]._IsActive = true;
            _Inputs[i].Dead += OnDead;
        }
    }

    private void OnDead(PlayerController player)
    {
        for (int i = 0; i < _Inputs.Count; i++)
        {
            if(player != _Inputs[i])
            {
                _Inputs[i].AddKill();
            }
        }

        _WillRespawn = true;
        _RespawnTime = 3f;
    }

    private float _RespawnTime = 3f;
    private void Update()
    {
        if(_WillRespawn)
        {
            _RespawnTime -= Time.deltaTime;

            if(_RespawnTime < 0)
            {
                _WillRespawn = false;
                for (int i = 0; i < _Inputs.Count; i++)
                {
                    _Inputs[i].Respawn();
                }
            }
        }

        if(!_HasGameStart)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                _CharacterCreator.LockIn(0);
            }

            if(Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                _CharacterCreator.LockIn(1);
            }
        }
    }
}
