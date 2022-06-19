using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sits on GameManager object.
/// Only for testing purposes
/// </summary>

public class TestButtons : MonoBehaviour
{
    public void EndTurn()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerTurnEnd);
    }

    public void DescribeScriptableObj()
    {
        foreach (CardScriptableObj cd in GameManager.Instance.baseCardsSO)
        {
            cd.DebugText();
        }
    }
}
