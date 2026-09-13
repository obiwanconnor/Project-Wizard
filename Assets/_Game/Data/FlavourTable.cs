using UnityEngine;

namespace WhereAreMyKeys.Data
{
    /// <summary>
    /// A pool of flavour lines for a searchable object (GDD section 3).
    /// Kid-written — import from a spreadsheet via
    /// <c>Where Are My Keys ▸ Import Flavour Text From CSV...</c> rather
    /// than typing lines into the Inspector one at a time.
    /// </summary>
    [CreateAssetMenu(menuName = "Where Are My Keys/Flavour Table", fileName = "FlavourTable")]
    public class FlavourTable : ScriptableObject
    {
        [TextArea(1, 3)]
        public string[] lines = System.Array.Empty<string>();

        public string GetRandomLine()
        {
            if (lines == null || lines.Length == 0) return string.Empty;
            return lines[Random.Range(0, lines.Length)];
        }
    }
}
