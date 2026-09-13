using UnityEngine;

namespace WhereAreMyKeys.Data
{
    /// <summary>
    /// One candle's half-remembered hint (GDD section 4). Unreliable by
    /// design — the design rule is that no memory names a key location
    /// outright.
    /// </summary>
    [CreateAssetMenu(menuName = "Where Are My Keys/Candle Memory", fileName = "CandleMemory")]
    public class CandleMemory : ScriptableObject
    {
        [TextArea(2, 4)] public string text;
    }
}
