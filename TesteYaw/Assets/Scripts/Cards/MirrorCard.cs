using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MirrorCard : Card
{
    public override void CardEffect(CurrentTurn currentTurn)
    {
        GameManager.Instance.SwapPoints();

        transform.DOMove(transform.position, 1).OnComplete(() =>
        {
            ChangeTurn(currentTurn);
        });
    }
}
