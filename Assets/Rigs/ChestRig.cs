using UnityEngine;
using Cainos.PixelArtPlatformer_Dungeon;

namespace WhereAreMyKeys.Rigs
{
    /// <summary>
    /// Same boundary as <see cref="PlayerRig"/> and <see cref="MonsterRig"/>,
    /// but unlike those two this one isn't a stub: the environment pack
    /// (unlike the character/monster packs) is already imported, so the
    /// real Cainos type is available to reference here.
    ///
    /// Cainos's Chest.cs is visuals-only (an IsOpened bool driving its own
    /// Animator via a public Open() method) — it has no interaction or
    /// trigger logic of its own. Our own <see cref="Interaction.Chest"/>
    /// owns the "was this searched" decision but can't reference the Cainos
    /// type directly (WhereAreMyKeys.asmdef), so this file bridges the two:
    /// when our Chest opens, tell the Cainos Chest to open too.
    ///
    /// Only needed on chest prefabs actually using Cainos's Chest art/anim
    /// (per the GDD, some searchables reuse barrel/crate art with their own
    /// bespoke Animator instead — those don't need this component).
    /// </summary>
    public class ChestRig : MonoBehaviour
    {
        [SerializeField] private Interaction.Chest gameChest;
        [SerializeField] private Chest visualChest;

        private void OnEnable()
        {
            if (gameChest != null) gameChest.OnOpened += HandleOpened;
        }

        private void OnDisable()
        {
            if (gameChest != null) gameChest.OnOpened -= HandleOpened;
        }

        private void HandleOpened()
        {
            if (visualChest != null) visualChest.Open();
        }
    }
}
