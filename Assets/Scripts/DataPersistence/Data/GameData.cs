using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;

    public GameData()
    {
        playerPosition = new(0f, 1f, 0f);
    }
}
