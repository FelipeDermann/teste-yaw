using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleCard : Card
{
    public override void CardEffect(CurrentTurn currentTurn)
    {
        int pointsToGive = currentTurn == CurrentTurn.PlayerTurn
            ? GameManager.Instance.PlayerPoints
            : GameManager.Instance.EnemyPoints;
        
        if(currentTurn == CurrentTurn.PlayerTurn) 
            GameManager.Instance.ChangePlayerPoints(pointsToGive);
        else GameManager.Instance.ChangeEnemyPoints(pointsToGive);
        
        ChangeTurn(currentTurn);
    }
}
