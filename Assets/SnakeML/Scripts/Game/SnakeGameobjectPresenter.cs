using UnityEngine;



// TODO: placeholder class/file for the game. The program structure will be refactored later
// MonoBehaviour for Instantiate
class SnakeGameobjectPresenter : MonoBehaviour 
{
    [SerializeField]
    private Material _emptyMat;

    [SerializeField]
    private Material _foodMat;

    [SerializeField]
    private Material _snakeHeadMat;

    [SerializeField]
    private Material _snakeBodyMat;

    [SerializeField]
    private GameObject _tile;

    private Renderer[,] renderGrid;

    public void Start()
    {
        // init grid
        renderGrid = new Renderer[GameState.WORLD_SIZE, GameState.WORLD_SIZE];
        float tilePositionIncrement = 0.5f;
        float positionOffset = -(GameState.WORLD_SIZE / 2) * tilePositionIncrement;
        Vector3 spawnPosition = new Vector3(positionOffset, positionOffset, 1);
        spawnPosition += transform.parent.position;
        for (int i = 0; i < GameState.WORLD_SIZE; i++)
        {
            for (int j = 0; j < GameState.WORLD_SIZE; j++)
            {
                renderGrid[i, j] = Instantiate(_tile, spawnPosition, Quaternion.identity, transform).GetComponent<Renderer>();
                spawnPosition.x += tilePositionIncrement;
            }
            spawnPosition.y += tilePositionIncrement;
            spawnPosition.x = positionOffset + transform.parent.position.x;
        }
    }

    public void Reset()
    {
        for (int i = 0; i < GameState.WORLD_SIZE; i++)
        {
            for (int j = 0; j < GameState.WORLD_SIZE; j++)
            {
                renderGrid[i, j].material = _emptyMat;
            }
        }

    }

    public void ChangeTileState(Grid.Index2D tileIndex, Grid.TileState state)
    {
        if (tileIndex.x >= GameState.WORLD_SIZE || tileIndex.y >= GameState.WORLD_SIZE || tileIndex.x < 0 || tileIndex.y < 0)
            return;

        Material material;

        switch (state)
        {
            case Grid.TileState.empty:
                material = _emptyMat;
                break;
            case Grid.TileState.food:
                material = _foodMat;
                break;
            case Grid.TileState.snake_body:
                material = _snakeBodyMat;
                break;
            case Grid.TileState.snake_head:
                material = _snakeHeadMat;
                break;
            default:
                return;
        }


        renderGrid[tileIndex.y, tileIndex.x].material = material;
    }
}
