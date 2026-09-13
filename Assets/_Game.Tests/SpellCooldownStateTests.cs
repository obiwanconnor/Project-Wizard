using NUnit.Framework;
using UnityEngine;
using WhereAreMyKeys.Combat;
using WhereAreMyKeys.Data;

namespace WhereAreMyKeys.Tests
{
    public class SpellCooldownStateTests
    {
        private static SpellDefinition MakeSpell(float cooldown)
        {
            var s = ScriptableObject.CreateInstance<SpellDefinition>();
            s.cooldownSeconds = cooldown;
            return s;
        }

        [Test]
        public void StartsReady()
        {
            var state = new SpellCooldownState(MakeSpell(3f));
            Assert.IsTrue(state.IsReady);
        }

        [Test]
        public void CastingStartsCooldown()
        {
            var state = new SpellCooldownState(MakeSpell(3f));
            Assert.IsTrue(state.TryCast());
            Assert.IsFalse(state.IsReady);
        }

        [Test]
        public void CannotCastAgainBeforeCooldownElapses()
        {
            var state = new SpellCooldownState(MakeSpell(3f));
            state.TryCast();
            state.Tick(1.5f);
            Assert.IsFalse(state.TryCast());
        }

        [Test]
        public void ReadyAgainAfterCooldownElapses()
        {
            var state = new SpellCooldownState(MakeSpell(3f));
            state.TryCast();
            state.Tick(3.1f);
            Assert.IsTrue(state.IsReady);
        }
    }
}
