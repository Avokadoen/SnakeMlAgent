using System.Collections.Generic;
using UnityEngine;
using MLAgents;

[RequireComponent(typeof(SnakeGameobjectPresenter))]
class GameState : MonoBehaviour
{

    // Todo: change this as other classes need this to be parameters
    public const int WORLD_SIZE = 25; 

    private Snake snake;
    private Grid grid;

    [SerializeField]
    private SnakeGameobjectPresenter snakeGameRenderer; // TODO: Type T with interface

    [SerializeField]
    private Agent agent;

    private float sinceLastUpdate;
    public float score;
    private List<Grid.Index2D> foodsInGrid = new List<Grid.Index2D>(); // TODO allow for multiple foods being active

    [SerializeField]
    private float gameSpeed = 0.1f;

    public void Start()
    {
        snake = new Snake();
        snake.Init();

        grid = new Grid();
        if(transform.parent)
            grid.parentPos = transform.parent.position; // TODO: better solution!
        grid.Init();

        if (!snakeGameRenderer)
        {
            snakeGameRenderer.GetComponent<SnakeGameobjectPresenter>();
        }

        if (!agent)
        {
            Debug.Log("agent is null!");
        }
    }


    void Update()
    {
        if (snake.IsDead())
        {
            Reset();
            agent.Done();
        }

        sinceLastUpdate += Time.deltaTime;

        if (sinceLastUpdate > gameSpeed)
        {
            sinceLastUpdate = 0;

            if (foodsInGrid.Count <= 0)
            {
                SpawnFood();
            }

            {
                var snakeTail = snake.bodyPositions[snake.bodyPositions.Count - 1];
                ChangeTileState(snakeTail, Grid.TileState.empty);
            }

            snake.Update();
            agent.SetReward(snake.EatFood(ref foodsInGrid));

            ChangeTileState(snake.headPosition, Grid.TileState.snake_head);
        }
    }

    public void Reset()
    {
        snake.Reset();
        grid.Reset();
        snakeGameRenderer.Reset();

        foodsInGrid.Clear();
    }

    private void ChangeTileState(Grid.Index2D index, Grid.TileState state)
    {
        snakeGameRenderer.ChangeTileState(index, state);
        grid.ChangeTileState(index, state);
    }


    public void ChangeSnakeDirection(Snake.Direction direction)
    {
        snake.direction = direction;
    }

    // TODO: rectangle input (grid state compression) 
    public float[] GetObservationVector()
    {
        var observations = new float[20];
        observations[0] = snake.headPosition.x;
        observations[1] = snake.headPosition.y;
        if(foodsInGrid.Count > 1)
        {
            observations[2] = snake.headPosition.x - foodsInGrid[0].x;
            observations[3] = snake.headPosition.y - foodsInGrid[0].y;
        }

        var compressedGrid = grid.CompressGridData();
        var obsIndex = 4;
        for(int i = 0; i < 8; i++)
        {
            observations[obsIndex++] = compressedGrid[i].x;
            observations[obsIndex++] = compressedGrid[i].y;
        }

        return observations;
    }

    private Grid.Index2D SpawnFood()
    {
        var foodIndex = grid.GetValidSpawnPosition();
        ChangeTileState(foodIndex, Grid.TileState.food);
        foodsInGrid.Add(foodIndex);
        return foodIndex;
    }
}

