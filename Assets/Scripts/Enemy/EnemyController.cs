using Hero;
using States;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class EnemyController : CharacterController
    {
        public EnemyHealth enemyHealth;

        [SerializeField]
        private BoxCollider2D boxCollider2D;
        public BoxCollider2D BoxCollider2D => boxCollider2D;

        public DamageType enemyDamageType;

        public Image enemyDamageTypeIcon;

        public int enemyLevel;

        private void Start()
        {
            HeroAttack.OnInflictDamage += TakeDamage;
        }

        private void TakeDamage(float attackPoint)
        {
            enemyHealth.SetHealth(attackPoint);

            var hitState = GetState(StateType.Hit);
            TransitionToState(hitState);
        }

        private void OnDestroy()
        {
            HeroAttack.OnInflictDamage -= TakeDamage;
        }
    }
}