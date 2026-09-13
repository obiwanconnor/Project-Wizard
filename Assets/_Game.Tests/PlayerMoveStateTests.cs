using NUnit.Framework;
using UnityEngine;
using WhereAreMyKeys.Movement;

namespace WhereAreMyKeys.Tests
{
    public class PlayerMoveStateTests
    {
        [Test]
        public void StartsAtGivenPosition()
        {
            var state = new PlayerMoveState(4f, new Vector2(1f, 2f));
            Assert.AreEqual(new Vector2(1f, 2f), state.Position);
        }

        [Test]
        public void MovesInInputDirectionAtGivenSpeed()
        {
            var state = new PlayerMoveState(4f);
            state.Tick(Vector2.right, 1f);
            Assert.AreEqual(new Vector2(4f, 0f), state.Position);
        }

        [Test]
        public void ZeroInputDoesNotMove()
        {
            var state = new PlayerMoveState(4f, new Vector2(5f, 5f));
            state.Tick(Vector2.zero, 1f);
            Assert.AreEqual(new Vector2(5f, 5f), state.Position);
        }

        [Test]
        public void DiagonalArrowKeyInputIsClampedToUnitLength()
        {
            // A raw two-key composite (e.g. up+right arrow) reports (1,1)
            // — length sqrt(2) — but diagonal movement must be no faster
            // than a single axis.
            var state = new PlayerMoveState(4f);
            state.Tick(new Vector2(1f, 1f), 1f);
            Assert.AreEqual(4f, state.Position.magnitude, 0.0001f);
        }

        [Test]
        public void PartiallyTiltedGamepadStickMovesProportionallySlower()
        {
            // A gamepad left stick pushed only halfway should move at
            // half speed, not get clamped up to full speed.
            var state = new PlayerMoveState(4f);
            state.Tick(new Vector2(0.5f, 0f), 1f);
            Assert.AreEqual(new Vector2(2f, 0f), state.Position);
        }

        [Test]
        public void AccumulatesAcrossMultipleTicks()
        {
            var state = new PlayerMoveState(2f);
            state.Tick(Vector2.up, 0.5f);
            state.Tick(Vector2.up, 0.5f);
            Assert.AreEqual(new Vector2(0f, 2f), state.Position);
        }
    }
}
