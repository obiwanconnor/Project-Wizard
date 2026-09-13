using UnityEngine;
using WhereAreMyKeys.GameState;

namespace WhereAreMyKeys.UI
{
    /// <summary>
    /// Keys hang visibly on the wizard's belt as he collects them (GDD
    /// section 8) — this just toggles the belt icons; KeyRing owns the count.
    /// </summary>
    public class KeyBeltDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject[] keyIcons;

        private void OnEnable()
        {
            if (KeyRing.Instance != null)
            {
                KeyRing.Instance.OnKeyCountChanged += Refresh;
                Refresh(KeyRing.Instance.KeysCollected);
            }
        }

        private void OnDisable()
        {
            if (KeyRing.Instance != null)
                KeyRing.Instance.OnKeyCountChanged -= Refresh;
        }

        private void Refresh(int collected)
        {
            if (keyIcons == null) return;
            for (int i = 0; i < keyIcons.Length; i++)
                keyIcons[i].SetActive(i < collected);
        }
    }
}
