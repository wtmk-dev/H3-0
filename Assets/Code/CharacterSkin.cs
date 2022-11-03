using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkin : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _Body, _Head, _Feet;

    public void Skin(List<SpriteRenderer> colors)
    {
        _Body.color = colors[0].color;
        _Head.color = colors[1].color;
        _Feet.color = colors[2].color;
    }
}
