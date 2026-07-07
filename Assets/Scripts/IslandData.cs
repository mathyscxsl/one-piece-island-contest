using UnityEngine;

[CreateAssetMenu(fileName = "IslandData", menuName = "One Piece/Island Data")]
public class IslandData : ScriptableObject
{
    [Header("Identity")]
    public string islandName;

    [Header("Enemies")]
    public GameObject monsterPrefab;
    public GameObject bossPrefab;
    public int monsterCount = 3;
}
