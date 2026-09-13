using NUnit.Framework;
using WhereAreMyKeys.Combat;

namespace WhereAreMyKeys.Tests
{
    public class HealthStateTests
    {
        [Test]
        public void StartsAtMaxHeartsAndAlive()
        {
            var health = new HealthState(3, invulnerabilitySeconds: 0.6f);
            Assert.AreEqual(3, health.CurrentHearts);
            Assert.IsFalse(health.IsDead);
        }

        [Test]
        public void TakeDamageReducesHearts()
        {
            var health = new HealthState(3, 0.6f);
            Assert.IsTrue(health.TakeDamage(1, currentTime: 0f));
            Assert.AreEqual(2, health.CurrentHearts);
        }

        [Test]
        public void TakeDamageIgnoredWhenAmountIsNotPositive()
        {
            var health = new HealthState(3, 0.6f);
            Assert.IsFalse(health.TakeDamage(0, 0f));
            Assert.IsFalse(health.TakeDamage(-1, 0f));
            Assert.AreEqual(3, health.CurrentHearts);
        }

        [Test]
        public void TakeDamageBlockedDuringInvulnerabilityWindow()
        {
            var health = new HealthState(3, invulnerabilitySeconds: 1f);
            health.TakeDamage(1, currentTime: 0f);
            Assert.IsFalse(health.TakeDamage(1, currentTime: 0.5f));
            Assert.AreEqual(2, health.CurrentHearts);
        }

        [Test]
        public void TakeDamageAllowedAfterInvulnerabilityWindowElapses()
        {
            var health = new HealthState(3, invulnerabilitySeconds: 1f);
            health.TakeDamage(1, currentTime: 0f);
            Assert.IsTrue(health.TakeDamage(1, currentTime: 1.1f));
            Assert.AreEqual(1, health.CurrentHearts);
        }

        [Test]
        public void DiesWhenHeartsReachZero()
        {
            var health = new HealthState(2, 0f);
            health.TakeDamage(2, currentTime: 0f);
            Assert.AreEqual(0, health.CurrentHearts);
            Assert.IsTrue(health.IsDead);
        }

        [Test]
        public void DamageNeverDropsHeartsBelowZero()
        {
            var health = new HealthState(2, 0f);
            health.TakeDamage(99, currentTime: 0f);
            Assert.AreEqual(0, health.CurrentHearts);
        }

        [Test]
        public void CannotTakeDamageAfterDeath()
        {
            var health = new HealthState(1, 0f);
            health.TakeDamage(1, currentTime: 0f);
            Assert.IsTrue(health.IsDead);
            Assert.IsFalse(health.TakeDamage(1, currentTime: 1f));
        }

        [Test]
        public void HealRestoresHearts()
        {
            var health = new HealthState(3, 0.6f);
            health.TakeDamage(2, currentTime: 0f);
            Assert.IsTrue(health.Heal(1));
            Assert.AreEqual(2, health.CurrentHearts);
        }

        [Test]
        public void CannotHealPastMaxHearts()
        {
            var health = new HealthState(3, 0.6f);
            Assert.IsFalse(health.Heal(1));
            Assert.AreEqual(3, health.CurrentHearts);
        }

        [Test]
        public void CannotHealAfterDeath()
        {
            var health = new HealthState(1, 0f);
            health.TakeDamage(1, currentTime: 0f);
            Assert.IsFalse(health.Heal(1));
            Assert.AreEqual(0, health.CurrentHearts);
        }

        [Test]
        public void ResetToFullClearsDeathAndRestoresHearts()
        {
            var health = new HealthState(2, 0f);
            health.TakeDamage(2, currentTime: 0f);
            health.ResetToFull();
            Assert.IsFalse(health.IsDead);
            Assert.AreEqual(2, health.CurrentHearts);
        }
    }
}
