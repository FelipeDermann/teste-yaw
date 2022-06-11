using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BoltCard : Card
{
    public override void CardEffect(CurrentTurn currentTurn)
    {
        Card lastCardPlayed = currentTurn == CurrentTurn.PlayerTurn ? 
            CardManager.Instance.EnemyLastCardDisposed : CardManager.Instance.PlayerLastCardDisposed;

        int pointsToReduce = lastCardPlayed.GetValue();

        if (lastCardPlayed.GetComponent<DoubleCard>())
            pointsToReduce = currentTurn == CurrentTurn.PlayerTurn
                ? GameManager.Instance.EnemyPoints/2
                : GameManager.Instance.PlayerPoints/2;
        
        CardManager.Instance.PlayBoltEffect(lastCardPlayed.transform.position);
        lastCardPlayed.gameObject.SetActive(false);

        if (currentTurn == CurrentTurn.PlayerTurn)
            GameManager.Instance.ChangeEnemyPoints(-pointsToReduce);
        else GameManager.Instance.ChangePlayerPoints(-pointsToReduce);
        
        //I used tween to just wait a bit before continuing so the gameplay looks better
        lastCardPlayed.transform.DOMove(transform.position, 1).OnComplete(() =>
        {
            ChangeTurn(currentTurn);
        });
    }
}
