// This file just holds our categories so we don't make typos.

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum ItemType
{
    Unit,       // Soldiers (Slinger, Spear, etc.)
    Material,   // Things to merge with (Stone, Scope, etc.)
    Buffer      // Stat boosters (War Drum, Banner)
}

public enum TargetType
{
    None,       // For materials
    Ground,     // For melee
    Air,        // For ranged (optional)
    Both
}