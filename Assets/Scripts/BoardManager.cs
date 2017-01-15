using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

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
     
    public int columns = 32;
    public int rows = 32;
    public Count wallCount = new Count(15, 30);
    public Count foodCount = new Count(7, 14);
    public Count decorative = new Count(4, 8);

    public GameObject exit;

    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] decorativeTiles;
    //public GameObject[] outerWallTiles;

    public GameObject botOuterWall;
    public GameObject topOuterWall;
    public GameObject leftOuterWall;
    public GameObject rightOuterWall;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitializeList()
    {
        gridPositions.Clear();
        // initialize the grid with default values
        for (int x = 1; x < columns -1; x++)
        {
            for(int y =1; y < rows -1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        // go to all the grid positions
        for(int x =-1; x < columns +1; x++)
        {
            for(int y = -1; y < rows +1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];


                // if we are on a outer wall
                if (x == -1)
                {
                    // left outer wall
                    toInstantiate = leftOuterWall;
                }
                if (x == columns)
                {
                    // right outer wall
                    toInstantiate = rightOuterWall;
                }
                if (y == -1)
                {
                    // bottom outer wall
                    toInstantiate = botOuterWall;
                }
                if (y == rows)
                {
                    // top outer wall
                    toInstantiate = topOuterWall;
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    /**
     *  Generate a random position, and remove it form the grid to be sure we don't place too many
     *  objects on the same tiles
     **/
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for(int i = 0; i < objectCount; i++)
        {
            // tiles positio
            Vector3 randomPosition = RandomPosition();
            // random tiles
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            // place the tiles
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    // only public function
    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        LayoutObjectAtRandom(decorativeTiles, decorative.minimum, decorative.maximum);

        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);


        Instantiate(exit, RandomPosition(), Quaternion.identity);
    }
}