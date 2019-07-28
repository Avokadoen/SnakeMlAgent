using System.Collections.Generic;
using UnityEngine;

// TODO: max size for grid
class Grid : IGameLogic
{
    public enum TileState { empty, snake_body, snake_head, food }
    public struct Tile
    {
        public TileState tileState;
        public Vector2 position;
    }
    public struct Index2D
    {
        public int y;
        public int x;
    }

    private Tile[,] grid;
    // I would like a hashset for optimal lookup, but it doesn't allow preallocation in current version
    private List<Index2D> emptyTilesIndex;
    public Vector3 parentPos;

    public void Init()
    {
        // init grid
        grid = new Tile[GameState.WORLD_SIZE, GameState.WORLD_SIZE];
        emptyTilesIndex = new List<Index2D>(GameState.WORLD_SIZE * GameState.WORLD_SIZE);
        float tilePositionIncrement = 0.5f;
        float positionOffset = -(GameState.WORLD_SIZE / 2) * tilePositionIncrement;
        Vector3 spawnPosition = new Vector2(positionOffset, positionOffset);
        spawnPosition += parentPos;
        for (int i = 0; i < GameState.WORLD_SIZE; i++)
        {
            for (int j = 0; j < GameState.WORLD_SIZE; j++)
            {
                emptyTilesIndex.Add(new Index2D { y = i, x = j });
                grid[i, j] = new Tile
                {
                    tileState = TileState.empty,
                    position = spawnPosition,
                };
                spawnPosition.x += tilePositionIncrement;
            }
            spawnPosition.y += tilePositionIncrement;
            spawnPosition.x = positionOffset + parentPos.x;
        }
    }

    public void Reset()
    {
        emptyTilesIndex.Clear();
        emptyTilesIndex.Capacity = GameState.WORLD_SIZE * GameState.WORLD_SIZE;
        for (int i = 0; i < GameState.WORLD_SIZE; i++)
        {
            for (int j = 0; j < GameState.WORLD_SIZE; j++)
            {
                emptyTilesIndex.Add(new Index2D { y = i, x = j });
                grid[i, j].tileState = TileState.empty;
            }
        }
    }

    public Index2D GetValidSpawnPosition()
    {
        var validSpawnIndex = Random.Range(0, emptyTilesIndex.Count - 1);
        var validSpawnPosition = emptyTilesIndex[validSpawnIndex];
        emptyTilesIndex.RemoveAt(validSpawnIndex);
        return validSpawnPosition;
    }

    public void Update()
    {
        // NOP for now
    }

    public void ChangeTileState(Index2D tileIndex, TileState state)
    {
        if (tileIndex.x >= GameState.WORLD_SIZE || tileIndex.y >= GameState.WORLD_SIZE || tileIndex.x < 0 || tileIndex.y < 0)
            return;


        grid[tileIndex.y, tileIndex.x].tileState = state;
    }

    public Index2D[] CompressGridData() // TODO: rename function
    {
        // we use 16 as we are limited to fixed size for the network input
        var compressedEmptyData = new Index2D[8];
        for(int i = 0; i < 8; i++)
        {
            compressedEmptyData[i].x = compressedEmptyData[i].y = -1;
        }

        int dataIndex = 0;
        Index2D prevTile = emptyTilesIndex[0];
        foreach (var tile in emptyTilesIndex)
        {
            if(dataIndex % 2 == 0)
            {
                var indexDiffX = Mathf.Abs(tile.x - prevTile.x);
                var indexDiffY = Mathf.Abs(tile.y - prevTile.y);
                if (indexDiffX >= 2 && indexDiffX < GameState.WORLD_SIZE-1 || indexDiffY >= 2 && indexDiffY < GameState.WORLD_SIZE - 1)
                {
                    compressedEmptyData[dataIndex] = tile;
                    dataIndex++;
                }
            }
            else
            {
                if (compressedEmptyData[dataIndex].x == -1 || compressedEmptyData[dataIndex].y == -1)
                {
                    compressedEmptyData[dataIndex] = tile;
                    dataIndex++;
                }
            }
            prevTile = tile;
        }

        return compressedEmptyData;
    }
}

