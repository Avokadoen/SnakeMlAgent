using MLAgents;
using UnityEngine;

[RequireComponent(typeof(GameState))]
class SnakeAgent : Agent
{
    public GameState gameState;
    public int NorthCounter;
    public int SouthCounter;
    public int WestCounter;
    public int EastCounter;

    void Start()
    {
        if (!gameState)
        {
            gameState.GetComponent<GameState>();
        }
    }

    public override void AgentReset()
    {
        gameState.Reset();
    }

    public override void CollectObservations()
    {
        AddVectorObs(gameState.GetObservationVector());
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // 0 = North | 1 = South ... etc
        var action = (int)Mathf.Abs(vectorAction[0]) * 4;
        if (Mathf.Abs(vectorAction[0]) > 1 || Mathf.Abs(vectorAction[0]) < 0) Debug.LogError("invalid action: " + Mathf.Abs(vectorAction[0]));

        Snake.Direction direction = (Snake.Direction)(Mathf.Abs(vectorAction[0]) * 4);
        gameState.ChangeSnakeDirection(direction);
    }
}
