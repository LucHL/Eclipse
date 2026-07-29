using System.Collections.Generic;
using UnityEngine;

public enum RelationType
{
    // Just meet
    Acquaintance,

    // Family
    Father,
    Mother,
    Son,
    Daughter,
    Brother,
    Sister,

    Grandfather,
    Grandmother,
    Grandson,
    Granddaughter,
    Uncle,
    Aunt,
    Cousin,

    // Friend / Romance
    Friend,
    Crush,
    Fiance,
    Spouse,

    // Hostility
    Enemy,
}

public enum Jobs
{
    // Army
    Soldier,
}

public enum SocialClass
{
    // Classe Populaire / Basse
    Serf,
    Peasant,
    Beggar,
    Outcast,

    // Classe Moyenne / Artisans
    Tradesman,
    Merchant,
    Clergy,
    Soldier,

    // Classe Aisée / Noblesse
    Nobility,
    Royalt
}

public enum MilitaryStatus
{
    Civic,          // Civil
    Soldier,
    Knight,
    Commander
}

public enum Race
{
    // Peaceful
    Elf,

    // Neutral
    Human,
    Dwarf,

    // Non-Peaceful
    Orc,
    Goblin,
    Undead,
    Demon
}

public enum Gender
{
    Male,
    Female
}


public static class FactionSystem
{
    public enum Faction
    {
        Civilian,   // PNJ pacifiques
        Guards,     // Can attack monster
        Monsters, // Gobelins, Orc...
        Bandits     // Attack civils and guards
    }

    /// <summary>
    /// Return TRUE if the 'factionB' is hostile toward 'factionA'
    /// </summary>
    public static bool IsHostile(Faction factionA, Faction factionB)
    {
        if (factionA == factionB)
            return false;

        return factionA switch {
            Faction.Monsters => factionB == Faction.Civilian || factionB == Faction.Guards || factionB == Faction.Bandits,
            Faction.Guards => factionB == Faction.Monsters || factionB == Faction.Bandits,
            Faction.Civilian => false,
            Faction.Bandits => factionB == Faction.Civilian || factionB == Faction.Guards,
            _ => false,
        };
    }
}

[System.Serializable]
public class NPCRelation
{
    public string targetNPCId;
    public RelationType relationType;
}

[System.Serializable]
public class NPCIdentity
{
    [Header("Base Info")]
    public string npcId = null;     // ID unique (ex: "npc_0001_d4ds65cs")
    public string firstName = null;
    public string familyName = null;
    public int age;
    public bool isAlive = true;

    [Header("Position & World")]
    public Vector3 position;
    public Vector3 rotation;
    public string homeVillage;   // (ex: "village_Thann")
    public string currentVillage;

    [Header("Characteristic")]
    public Race race;
    public FactionSystem.Faction faction;
    public Gender gender;
    public string religion;
    public MilitaryStatus militaryStatus;
    public SocialClass socialClass;
    public Jobs jobs;

    [Header("Relations & Family")]
    public string fatherId;
    public string motherId;
    public string partner;
    public List<string> childrenIds = new();
    public List<NPCRelation> relations = new();
}
