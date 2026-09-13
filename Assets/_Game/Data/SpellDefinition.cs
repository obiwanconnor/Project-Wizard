using UnityEngine;
using WhereAreMyKeys.Combat;

namespace WhereAreMyKeys.Data
{
    /// <summary>
    /// A spell as data — projectile, cooldown, damage, cast sound — so a
    /// second spell later is a new asset, not new code (GDD section 10,
    /// "headroom worth protecting").
    ///
    /// NOTE: the wizard's own cast fires through the Cainos character
    /// controller's built-in Cast attack action (its own Projectile Prefab
    /// / Projectile Speed fields, plus <see cref="Rigs.SpellProjectileRig"/>
    /// for damage) rather than this asset's <see cref="projectilePrefab"/> /
    /// <see cref="projectileSpeed"/> — those two are unused on that path and
    /// exist for a future spell (or a monster's ranged attack) that fires
    /// our own <c>Projectile</c> directly instead of through a Cainos
    /// controller.
    /// </summary>
    [CreateAssetMenu(menuName = "Where Are My Keys/Spell Definition", fileName = "SpellDefinition")]
    public class SpellDefinition : ScriptableObject
    {
        public string spellName = "Bolt";
        public Projectile projectilePrefab;
        public float projectileSpeed = 9f;
        public float cooldownSeconds = 3f;
        public int damage = 1;
        public bool canLightCandles = true;
        public AudioClip castSound;
    }
}
