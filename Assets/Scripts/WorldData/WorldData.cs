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
public class RootDatabase
{
    public ReligionData Religions = new();
    public Dictionary<string, NamePoolGroup> NamePool = new();
}
