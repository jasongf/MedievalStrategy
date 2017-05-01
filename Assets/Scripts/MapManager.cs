using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
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

    public int columns = 10;
    public int rows = 10;
    public Count landCount = new Count(50, 75);
    public Count forestCount = new Count(30, 50);
    public Count rockCount = new Count(3, 6);
    public GameObject[] oceanTiles;
    public GameObject[] landTiles;
    public GameObject[] forestTiles;
    public GameObject[] beachTiles;
    public GameObject[] rockTiles;

    private Transform mapHolder;
    private List<Vector3> oceanPositions = new List<Vector3>();
    private List<Vector3> landPositions = new List<Vector3>();
    private List<Vector3> forestPositions = new List<Vector3>();
    private List<Vector3> rockPositions = new List<Vector3>();
    private List<Vector3> beachPositions = new List<Vector3>();

    void InitializeList(List<Vector3> gridPositions)
    {
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void MapSetup()
    {
        mapHolder = new GameObject("Map").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = oceanTiles[Random.Range(0, oceanTiles.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(mapHolder);
            }
        }
    }

    /// <summary>
    /// Returns a random position from our list gridPositions
    /// </summary>
    /// <returns></returns>
    Vector3 RandomPosition (List<Vector3> gridPositions)
    {
        //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
        int randomIndex = Random.Range(0, gridPositions.Count);

        //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
        Vector3 randomPosition = gridPositions[randomIndex];

        //Remove the entry at randomIndex from the list so that it can't be re-used.
        gridPositions.RemoveAt(randomIndex);

        //Return the randomly selected Vector3 position.
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum,List <Vector3>gridPositions, List<Vector3>objectPositions)
    {
        //Choose a random number of objects to instantiate within the minimum and maximum limits
        int objectCount = Random.Range(minimum, maximum + 1);

        //Instantiate objects until the randomly chosen limit objectCount is reached
        for (int i = 0; i < objectCount; i++)
        {
            //Choose a positiion for randomPosition by getting a random position from out list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition(gridPositions);

            // Add the randomPosition to the object positions
            objectPositions.Add(randomPosition);

            //Choose a random tile from tileArray and assign it to tileChoice
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    void LayoutBorder()
    {
        GameObject toInstantiate = null;
        GameObject toInstantiateBonus = null;
        List<Vector3> tempPositions = new List<Vector3>();
        bool isTop, isBottom, isRight, isLeft, isTopRight, isTopLeft, isBottomRight, isBottomLeft;

        foreach(Vector3 oceanPosition in oceanPositions)
        { 
            toInstantiate = null;
            toInstantiateBonus = null;

            isTop = landPositions.Contains(new Vector3(oceanPosition.x, oceanPosition.y + 1, 0f));
            isBottom = landPositions.Contains(new Vector3(oceanPosition.x, oceanPosition.y - 1, 0f));
            isRight = landPositions.Contains(new Vector3(oceanPosition.x + 1, oceanPosition.y, 0f));
            isLeft = landPositions.Contains(new Vector3(oceanPosition.x - 1, oceanPosition.y, 0f));
            isTopRight = landPositions.Contains(new Vector3(oceanPosition.x + 1, oceanPosition.y + 1, 0f));
            isTopLeft = landPositions.Contains(new Vector3(oceanPosition.x - 1, oceanPosition.y + 1, 0f));
            isBottomRight = landPositions.Contains(new Vector3(oceanPosition.x + 1, oceanPosition.y - 1, 0f));
            isBottomLeft = landPositions.Contains(new Vector3(oceanPosition.x - 1, oceanPosition.y - 1, 0f));
            
            // Outside
            if (isTopRight) toInstantiate = beachTiles[11];         // Top Right Outside
            if (isTopLeft) toInstantiate = beachTiles[9];           // Top Left Outside
            if (isBottomRight) toInstantiate = beachTiles[4];       // Bottom Right Outside
            if (isBottomLeft) toInstantiate = beachTiles[2];        // Bottom Left Outside

            // Opposite Corner
            if (isTopRight && isBottomLeft && !isTopLeft && !isBottomRight) { toInstantiate = beachTiles[11]; toInstantiateBonus = beachTiles[2]; }
            if (isTopLeft && isBottomRight && !isTopRight && !isBottomLeft) { toInstantiate = beachTiles[9]; toInstantiateBonus = beachTiles[4]; }

            // Side
            if (isTop) toInstantiate = beachTiles[7];               // Top
            if (isBottom) toInstantiate = beachTiles[0];            // Bottom
            if (isRight) toInstantiate = beachTiles[6];             // Right  
            if (isLeft) toInstantiate = beachTiles[5];              // Left

            // Inside
            if ((isTop && isRight) || (isTop && isBottomRight) || (isRight && isTopLeft)) toInstantiate = beachTiles[10];   // Top Right Inside
            if ((isTop && isLeft) || (isTop && isBottomLeft) || (isLeft && isTopRight)) toInstantiate = beachTiles[8];     // Top Left Inside
            if ((isBottom && isRight) || (isBottom && isTopRight) || (isRight && isBottomLeft)) toInstantiate = beachTiles[3]; // Bottom Right Inside
            if ((isBottom && isLeft) || (isBottom && isTopLeft) || (isLeft && isBottomRight)) toInstantiate = beachTiles[1];  // Bottom Left Inside

            // Instantiate
            if (toInstantiate != null)
            {
                GameObject instance = Instantiate(toInstantiate, new Vector3(oceanPosition.x, oceanPosition.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(mapHolder);
                tempPositions.Add(new Vector3(oceanPosition.x, oceanPosition.y, 0f));
            }

            if (toInstantiateBonus != null)
            {
                GameObject instance = Instantiate(toInstantiateBonus, new Vector3(oceanPosition.x, oceanPosition.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(mapHolder);
            }
        }

        foreach (Vector3 tempPosition in tempPositions)
        {
            oceanPositions.Remove(tempPosition);
            beachPositions.Add(tempPosition);
        }
    }

    bool FillInLand()
    {
        GameObject toInstantiate = null;
        List<Vector3> tempPositions = new List<Vector3>();
        bool landFilled = false;

        foreach(Vector3 oceanPosition in oceanPositions)
        {
            toInstantiate = landTiles[Random.Range(0, landTiles.Length)];

            if (TileSurrounded(oceanPosition, landPositions))
            {
                tempPositions.Add(oceanPosition);
                GameObject instance = Instantiate(toInstantiate, new Vector3(oceanPosition.x, oceanPosition.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(mapHolder);
                landFilled = true;
            }
        }

        foreach (Vector3 tempPosition in tempPositions)
        {
            oceanPositions.Remove(tempPosition);
            landPositions.Add(tempPosition);
        }

        return landFilled;
    }

    bool TileSurrounded(Vector3 surroundedPosition, List<Vector3>surroundingPositions)
    {
        bool isTop = surroundingPositions.Contains(new Vector3(surroundedPosition.x, surroundedPosition.y + 1, 0f));
        bool isBottom = surroundingPositions.Contains(new Vector3(surroundedPosition.x, surroundedPosition.y - 1, 0f));
        bool isRight = surroundingPositions.Contains(new Vector3(surroundedPosition.x + 1, surroundedPosition.y, 0f));
        bool isLeft = surroundingPositions.Contains(new Vector3(surroundedPosition.x - 1, surroundedPosition.y, 0f));
        bool isTopRight = surroundingPositions.Contains(new Vector3(surroundedPosition.x + 1, surroundedPosition.y + 1, 0f));
        bool isTopLeft = surroundingPositions.Contains(new Vector3(surroundedPosition.x - 1, surroundedPosition.y + 1, 0f));
        bool isBottomRight = surroundingPositions.Contains(new Vector3(surroundedPosition.x + 1, surroundedPosition.y - 1, 0f));
        bool isBottomLeft = surroundingPositions.Contains(new Vector3(surroundedPosition.x - 1, surroundedPosition.y - 1, 0f));

        if ((isTop && isBottom) || (isRight && isLeft) || (isTopLeft && isRight && isBottom) 
            || (isTopRight && isLeft && isBottom)
            || (isBottomLeft && isRight && isTop) || (isBottomRight && isLeft && isTop))
        {
            return true;
        }
        return false;
    }

    public void SetupScene()
    {
        MapSetup();

        InitializeList(oceanPositions);

        // Layout land tiles on available ocean tiles
        LayoutObjectAtRandom(landTiles, landCount.minimum, landCount.maximum, oceanPositions, landPositions);

        // Fill in invalid spaces
        int fillCount = 0;
        bool mapNotFilled = true;
        while (mapNotFilled)
        {
            mapNotFilled = FillInLand();
            fillCount++;
        }
        Debug.Log(fillCount.ToString());

        //Expand boarder of ocean tiles by one
        for (int x = 0; x < columns; x++)
        {
            oceanPositions.Add(new Vector3(x, 0));
            oceanPositions.Add(new Vector3(x, rows - 1));
        }

        for (int y = 1; y < rows - 1; y++)
        {
            oceanPositions.Add(new Vector3(0, y));
            oceanPositions.Add(new Vector3(columns - 1, y));
        }

        // Suround land with beach tiles
        LayoutBorder();

        // Layout forests onto available land tiles
        LayoutObjectAtRandom(forestTiles, forestCount.minimum, forestCount.maximum, landPositions, forestPositions);

        // Layout rocks on available ocean tiles
        LayoutObjectAtRandom(rockTiles, rockCount.minimum, rockCount.maximum, oceanPositions, rockPositions);
    }
}
