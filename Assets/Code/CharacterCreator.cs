using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreator
{
    public event Action StartGame;

    public CharacterCreator(List<CharacterCreationView> creators)
    {

    }

    private bool LeftLock, RightLock;
    public void LockIn(int player)
    {
        if(player == 0)
        {
            LeftLock = true;
        }

        if(player == 1)
        {
            RightLock = true;
        }

        if(RightLock && LeftLock)
        {
            StartGame?.Invoke();
        }
    }
}
