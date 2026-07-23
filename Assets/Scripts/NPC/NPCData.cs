using System.Collections.Generic;

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

public enum HostilityLevel
{
    Peaceful,
    Neutral,
    Aggressive
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

public enum Religion
{
    None,
    Vraxxisme,
}


[System.Serializable]
public class NPCRelation
{
    public string targetNPCId;
    public RelationType relationType;
    // public int affinity;        // -100 (hate, want to kill) / +100 (love++)
}

[System.Serializable]
public class NPCIdentity
{
    public string npcId;           // ID unique (ex: "npc_Claude_01")
    public string firstName;
    public string familyName;
    public NPCDataSO baseData;

    public List<NPCRelation> relations = new();
}
