using System;
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


[System.Serializable]
public class NPCRelation
{
    public string targetNPCId;
    public RelationType relationType;
}

[System.Serializable]
public class NPCIdentity
{
    // NPC Info
    public string npcId = null;     // ID unique (ex: "npc_Claude_01")
    public string firstName = null;
    public string familyName = null;
    public int age;
    public bool isAlive = true;
    public Vector3 position;
    public Vector3 rotation;

    public Race race;
    public Gender gender;
    public string religion;
    public MilitaryStatus militaryStatus;
    public SocialClass socialClass;
    public Jobs jobs;

    // Village
    public string homeVillage;   // (ex: "village_Thann")
    public string currentVillage;

    // Other NPC
    public List<NPCRelation> relations = new();

    // Keep track of the family
    public Tuple<string, string> refToParent; // T1: father id, T2: mother id
    public string partner;
    public List<string> refToChildrens;
}
