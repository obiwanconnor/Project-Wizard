using NUnit.Framework;
using WhereAreMyKeys.GameState;

namespace WhereAreMyKeys.Tests
{
    public class KeyRingStateTests
    {
        [Test]
        public void StartsAtZero()
        {
            var ring = new KeyRingState(3);
            Assert.AreEqual(0, ring.KeysCollected);
            Assert.IsFalse(ring.HasAllKeys);
        }

        [Test]
        public void HasAllKeysAfterThree()
        {
            var ring = new KeyRingState(3);
            ring.AddKey();
            ring.AddKey();
            ring.AddKey();
            Assert.IsTrue(ring.HasAllKeys);
        }

        [Test]
        public void DoesNotExceedTotal()
        {
            var ring = new KeyRingState(3);
            for (int i = 0; i < 5; i++) ring.AddKey();
            Assert.AreEqual(3, ring.KeysCollected);
        }
    }
}
