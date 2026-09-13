using UnityEngine;
using WhereAreMyKeys.Data;
using WhereAreMyKeys.UI;

namespace WhereAreMyKeys.Interaction
{
    /// <summary>
    /// Pure flavour text, no gameplay effect — the cheapest way for the
    /// kids to add a joke (GDD section 7).
    /// </summary>
    public class Sign : Interactable
    {
        [SerializeField] private FlavourTable flavourTable;
        [SerializeField, TextArea(1, 3)] private string fixedText;

        protected override void OnInteract(GameObject interactor)
        {
            string text = flavourTable != null ? flavourTable.GetRandomLine() : fixedText;
            MemoryLog.Instance?.ShowText(text);
        }
    }
}
