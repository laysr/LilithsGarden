namespace LilithsHeart.Resources.Prefabs;

/// <summary>
/// Decorates a PrefabGUID field with a human-friendly alias (NewName) that will
/// be written into the generated Names JSON files by PrefabNamesExporter.
///
/// Keeping the alias co-located with the GUID field means:
///   - New entries need only one line with both pieces of data.
///   - Typos or renames are caught at the source, not in a separate lookup table.
///   - The exporter needs no external mapping file; reflection reads everything.
///
/// Usage:
///   [PrefabName("Smithy")]
///   public static readonly PrefabGUID TM_CraftingStation_Smithy = new(-1840926436);
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class PrefabNameAttribute : Attribute
{
    public string NewName { get; }

    public PrefabNameAttribute(string newName)
    {
        NewName = newName;
    }
}
