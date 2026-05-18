namespace LilithsCookbook.Data;

public class CookbookStationData
{
    public Dictionary<string, StationEntry> Stations { get; set; } = new();
}

public class StationEntry
{
    public bool ChangesEnabled { get; set; } = false;
    public List<string> AddRecipes { get; set; } = new();
    public List<string> RemoveRecipes { get; set; } = new();
}
