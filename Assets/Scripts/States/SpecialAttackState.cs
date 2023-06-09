using Enums;
using Hero;
using Managers;
using UI;
using UnityEngine;

namespace States
{
    public class SpecialAttackState : State
    {
        public State runState;
        public State idleState;

        [Header("Special Attack Prefabs")]
        public GameObject explosionAttackPrefab;

        public GameObject lightningAttackPrefab;

        public GameObject iceAttackPrefab;

        public GameObject holyAttackPrefab;

        protected override void EnterState()
        {
            ButtonController.OnActiveAttackButtons?.Invoke(false);

            CharacterController.AnimationController.PlayAnimation(AnimationType.SpecialAttack);

            CharacterController.AnimationController.onAnimationAction.AddListener(() =>
                SetSpecialAttackPrefab().SetActive(true));

            CharacterController.AnimationController.onAnimationAction.AddListener(() =>
                HeroAttack.OnInflictDamage?.Invoke(
                    GameManager.Instance.HeroController.heroAttack.GetSpecialAttackDamage(),
                    AttackType.SpecialAttackDamage));

            HeroController heroController = (HeroController) CharacterController;
            CharacterController.AnimationController.onAnimationEnd.AddListener(heroController.DecideNextState);

            base.EnterState();
        }

        protected override void ExitState()
        {
            CharacterController.AnimationController.onAnimationAction.RemoveAllListeners();
            CharacterController.AnimationController.onAnimationEnd.RemoveAllListeners();

            base.ExitState();
        }

        private GameObject SetSpecialAttackPrefab()
        {
            GameObject specialAttack = null;
            
            HeroController heroController = (HeroController) CharacterController;
            switch (heroController.heroAttack.specialAttackType)
            {
                case SpecialAttackType.Explosion:
                    specialAttack = explosionAttackPrefab;
                    break;
                case SpecialAttackType.Lightning:
                    specialAttack = lightningAttackPrefab;
                    break;
                case SpecialAttackType.IceAttack:
                    specialAttack = iceAttackPrefab;
                    break;
                case SpecialAttackType.Holy:
                    specialAttack = holyAttackPrefab;
                    break;
            }

            var position = specialAttack.transform.position;
            position = new Vector3(GameManager.Instance.EnemyController.specialAttackPosition.position.x,
                position.y, position.z);
            specialAttack.transform.position = position;

            return specialAttack;
        }
    }
}