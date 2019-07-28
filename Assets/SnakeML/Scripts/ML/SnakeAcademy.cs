using MLAgents;
using System.Collections.Generic;
using UnityEngine;

public class SnakeAcademy : Academy
{
    [SerializeField]
    private GameState gameState;

    public override void InitializeAcademy()
    {
        if(!gameState)
        {
            Debug.LogError("no game state in the academy!");
        }
    }

    public override void AcademyReset()
    {
        //gameState.Reset();
    }

}
