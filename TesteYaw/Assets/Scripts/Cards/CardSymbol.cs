using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSymbol : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _symbolSprite;

    private void Awake()
    {
        _symbolSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public SpriteRenderer GetSymbolSprite()
    {
        return _symbolSprite;
    }
}
