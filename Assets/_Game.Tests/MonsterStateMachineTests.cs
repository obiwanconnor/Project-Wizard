using NUnit.Framework;
using UnityEngine;
using WhereAreMyKeys.AI;
using WhereAreMyKeys.Data;

namespace WhereAreMyKeys.Tests
{
    public class MonsterStateMachineTests
    {
        private static EnemyBehaviourProfile MakeProfile()
        {
            var p = ScriptableObject.CreateInstance<EnemyBehaviourProfile>();
            p.noticeRadius = 5f;
            p.attackRange = 1f;
            p.loseInterestRadius = 8f;
            p.attackWindup = 0.2f;
            p.attackCooldown = 1f;
            p.canAttackWhileMoving = true;
            return p;
        }

        [Test]
        public void StartsIdle()
        {
            var fsm = new MonsterStateMachine(MakeProfile());
            Assert.AreEqual(MonsterState.Idle, fsm.State);
        }

        [Test]
        public void NoticesPlayerWithinRadius()
        {
            var fsm = new MonsterStateMachine(MakeProfile());
            fsm.Tick(0.1f, 3f, Vector2.right, false);
            Assert.AreEqual(MonsterState.Chase, fsm.State);
        }

        [Test]
        public void IgnoresPlayerOutsideRadius()
        {
            var fsm = new MonsterStateMachine(MakeProfile());
            fsm.Tick(0.1f, 50f, Vector2.right, false);
            Assert.AreEqual(MonsterState.Idle, fsm.State);
        }

        [Test]
        public void EntersAttackWithinAttackRange()
        {
            var fsm = new MonsterStateMachine(MakeProfile());
            fsm.Tick(0.1f, 3f, Vector2.right, false);   // Idle -> Chase
            fsm.Tick(0.1f, 0.5f, Vector2.right, false); // Chase -> Attack
            Assert.AreEqual(MonsterState.Attack, fsm.State);
        }

        [Test]
        public void DeadOverridesEverything()
        {
            var fsm = new MonsterStateMachine(MakeProfile());
            fsm.Tick(0.1f, 0.5f, Vector2.right, true);
            Assert.AreEqual(MonsterState.Dead, fsm.State);
        }
    }
}
