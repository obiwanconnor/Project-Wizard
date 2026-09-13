using UnityEngine;
using WhereAreMyKeys.GameState;

namespace WhereAreMyKeys.Interaction
{
    /// <summary>
    /// Shows keyholes filling in as keys are collected; opens and triggers
    /// the win state once all three are in (GDD sections 5 and 9).
    /// </summary>
    public class BigDoor : Interactable
    {
        [SerializeField] private GameObject[] keyholeFilledIcons;
        [SerializeField] private Animator animator;

        private void OnEnable()
        {
            if (KeyRing.Instance != null)
            {
                KeyRing.Instance.OnKeyCountChanged += RefreshKeyholes;
                RefreshKeyholes(KeyRing.Instance.KeysCollected);
            }
        }

        private void OnDisable()
        {
            if (KeyRing.Instance != null)
                KeyRing.Instance.OnKeyCountChanged -= RefreshKeyholes;
        }

        private void RefreshKeyholes(int collected)
        {
            if (keyholeFilledIcons == null) return;
            for (int i = 0; i < keyholeFilledIcons.Length; i++)
                keyholeFilledIcons[i].SetActive(i < collected);
        }

        protected override void OnInteract(GameObject interactor)
        {
            if (KeyRing.Instance == null || !KeyRing.Instance.HasAllKeys) return;
            animator?.SetTrigger("Open");
            GameFlowController.Instance?.Win();
        }
    }
}
