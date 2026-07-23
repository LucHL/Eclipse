using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCData", menuName = "NPC Data")]
public class NPCDataSO : ScriptableObject
{
    public string npcId;
    public string defaultFirstName;
    public string defaultFamilyName;
    public GameObject character;
    public float baseMovementSpeed = 3f;
}
