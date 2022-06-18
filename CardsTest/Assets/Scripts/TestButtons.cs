using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButtons : MonoBehaviour
{
    public void EndTurn()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerTurnEnd);
    }
    
    public void StartTurn()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerTurnPrep);
    }

    public void DescribeScriptableObj()
    {
        foreach (CardScriptableObj cd in GameManager.Instance.testCSO)
        {
            cd.DebugText();
        }
    }
}
