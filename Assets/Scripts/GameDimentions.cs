
using System;
using UnityEngine;

public static class GameDimentions
{
    public static float BottomOfScreen = -5.5f;
    public static float TopOfScreen = 7f;
    public static float RightSideOfScreen = 11f;
    public static float LeftSideOfScreen = -11f;

    public static float PlayerScreenLimit = 5f;
    public static float SpawnSidesLimiter = 3f;

    public static bool IsOutsideScreenBoundries(Transform transform)
    {
        return transform.position.y >= TopOfScreen
                || transform.position.x >= RightSideOfScreen
                || transform.position.x <= LeftSideOfScreen
                || transform.position.y <= BottomOfScreen;
    }

    public static Vector3 GetRandomEnemyStartPos()
    {
        float xPos = UnityEngine.Random.Range(GameDimentions.LeftSideOfScreen + SpawnSidesLimiter, GameDimentions.RightSideOfScreen - SpawnSidesLimiter);
        Vector3 posToSpawn = new Vector3(xPos, GameDimentions.TopOfScreen, 0);
        //Debug.Log($"Spawning Enemy at {posToSpawn}");
        return posToSpawn;
    }
}
