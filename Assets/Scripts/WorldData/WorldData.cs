using System;
using System.Collections.Generic;

[Serializable]
public class ReligionData
{
    public List<string> Name = new();
    public List<string> Practice = new();
}

[Serializable]
public class NamePoolGroup
{
    public List<string> MaleFirstNames = new();
    public List<string> FemaleFirstNames = new();
    public List<string> FamilyNames = new();
}

[Serializable]
public class NamePoolData
{
    public NamePoolGroup Human;
    public NamePoolGroup Elf;
    public NamePoolGroup Dwarf;
    public NamePoolGroup Orc;
    public NamePoolGroup Goblin;
    public NamePoolGroup Demon;

    public NamePoolGroup GetPoolForRace(Race race)
    {
        return race switch {
            Race.Human => Human,
            Race.Elf => Elf,
            Race.Dwarf => Dwarf,
            Race.Orc => Orc,
            Race.Goblin => Goblin,
            Race.Demon => Demon,
            _ => Human
        };
    }
}

[Serializable]
public class RootDatabase
{
    public ReligionData Religions = new();
    public NamePoolData NamePool = new();
}
