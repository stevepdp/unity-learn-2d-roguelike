using Random = UnityEngine.Random;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9);                           // Minimum of 5 walls per level, maximum of 9
    public Count foodCount = new Count(5, 9);                           // Minimum of 5 walls per level, maximum of 9
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;                                      // Keeps the hierarchy clean
    private List<Vector3> gridPositions = new List<Vector3>();          /* Track all possible positions on our game board and keep track
                                                                           of whether or not an object has been spawned in that position */

    void InitialiseList()
    {
        gridPositions.Clear();

        /* Create a list of grid positions to place walls, enemies or pickups.
           Starting with the x axis. columns - 1 leaves an empty wall around
           that we don't create completely inpassable levels. */
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows -1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // Configures the outer wall and floor of our game board
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        // x = -1 here because we're building the coliding wall around the playable area
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows - 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                // 0f on z because we're working in 2D. Quaternion.identity evaluates to no rotation
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex); // Remove this grid position from list so we don't spawn two objects at the same location
        return randomPosition;
    }

    // Spawns tiles at the random position we've already chosen
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1); // how many of a given object we're going to spawn, ie: walls.

        for (int i = 0; i < objectCount; i++) {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    // Called by the GameManager when it's time to set up the board
    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int) Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
    }
}
