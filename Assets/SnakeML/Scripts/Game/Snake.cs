using System.Collections.Generic;

class Snake : IGameLogic
{
    public enum Direction { North, South, West, East }

    public Grid.Index2D headPosition; // TODO: change to tile's
    public Grid.Index2D prevTailPosition;
    public List<Grid.Index2D> bodyPositions = new List<Grid.Index2D>();

    private float score;
    public float Score
    {
        get => score;
    }

    public Direction direction;

    public void Update()
    {

        if (bodyPositions.Count > 0)
            prevTailPosition = bodyPositions[bodyPositions.Count - 1];

        switch (direction)
        {
            case Direction.North:
                headPosition.y += 1;
                break;
            case Direction.South:
                headPosition.y -= 1;
                break;
            case Direction.West:
                headPosition.x -= 1;
                break;
            case Direction.East:
                headPosition.x += 1;
                break;
        }

        for (int i = bodyPositions.Count - 1; i >= 1; i--)
        {
            bodyPositions[i] = bodyPositions[i - 1];
        }

        if (IsDead()) return;

        bodyPositions[0] = headPosition;

    }

    public bool IsDead()
    {
        var currentHeadPos = headPosition;
        return (headPosition.x >= GameState.WORLD_SIZE
            || headPosition.y >= GameState.WORLD_SIZE
            || headPosition.x < 0
            || headPosition.y < 0
            || bodyPositions.FindAll(part => part.x == currentHeadPos.x && part.y == currentHeadPos.y).Count > 1);
    }

    public float EatFood(ref List<Grid.Index2D> foodPos)
    {
        if (foodPos.Remove(headPosition))
        {
            score += 0.1f; // TODO: remove score
            bodyPositions.Add(prevTailPosition);
            return 0.1f;
        }
        return 0f;
    }

    public void Init()
    {
        Reset();
    }

    public void Reset()
    {
        headPosition.x = GameState.WORLD_SIZE / 2;
        headPosition.y = GameState.WORLD_SIZE / 2;

        direction = Direction.East;

        bodyPositions.Clear();
        bodyPositions.Add(headPosition);
    }
}