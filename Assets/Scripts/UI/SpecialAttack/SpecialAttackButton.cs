using System;
using System.Threading;
using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using Enums;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SpecialAttack
{
    public class SpecialAttackButton : MonoBehaviour
    {
        public GameObject lockBackground;
        public Color baseBorderColor;
        public Button buttonComponent;
        public Image buttonBackground;
        public Image outerCircleImage;
        public UIShiny outerCircleShine;
        public Image sliderImage;
        public TextMeshProUGUI timeText;
        public UIShiny iconShine;

        private SpecialAttackButtonState _specialAttackButtonState;
        public SpecialAttackButtonState SpecialAttackButtonState => _specialAttackButtonState;

        private void Start()
        {
            baseBorderColor = buttonBackground.color;
            outerCircleImage.color = baseBorderColor;
            buttonBackground.color = Color.grey;
        }

        public async UniTask StartDurationState(int remainingTime, int maximumTime,
            CancellationTokenSource cancellationTokenSource)
        {
            _specialAttackButtonState = SpecialAttackButtonState.OnPressed;

            buttonComponent.enabled = false;
            outerCircleImage.fillClockwise = false;

            outerCircleShine.effectPlayer.loop = true;
            outerCircleShine.effectPlayer.play = true;

            var passingTime = 100;
            var currentTime = maximumTime - remainingTime;

            while (remainingTime > 0)
            {
                remainingTime -= passingTime;
                SetDurationState(currentTime, maximumTime);
                await UniTask.Delay(passingTime);
                currentTime += passingTime;
            }

            outerCircleShine.effectPlayer.loop = false;
            outerCircleShine.effectPlayer.play = false;
            outerCircleShine.effectFactor = 1;

            cancellationTokenSource.Cancel();
        }

        private void SetDurationState(int currentTime, int maximumTime)
        {
            outerCircleImage.fillAmount = 1 - (float) currentTime / maximumTime;
            maximumTime -= currentTime;
            maximumTime /= 1000;

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(maximumTime);
            timeText.text = $"{timeSpan.Milliseconds}";
        }

        public async UniTask StartCoolDownState(int remainingTime, int maximumTime,
            CancellationTokenSource cancellationTokenSource)
        {
            _specialAttackButtonState = SpecialAttackButtonState.OnCoolDown;

            buttonComponent.enabled = false;

            sliderImage.fillAmount = 1;
            sliderImage.fillClockwise = false;
            sliderImage.gameObject.SetActive(true);

            outerCircleImage.fillClockwise = true;

            var passingTime = 100;
            var currentTime = maximumTime - remainingTime;

            while (remainingTime > 0 && cancellationTokenSource.IsCancellationRequested == false)
            {
                remainingTime -= passingTime;
                SetCoolDownState(currentTime, maximumTime);
                await UniTask.Delay(passingTime);
                currentTime += passingTime;
            }

            _specialAttackButtonState = SpecialAttackButtonState.OnReady;

            outerCircleImage.fillAmount = 1;
            DisableSliderImage();
            buttonComponent.enabled = true;
            iconShine.Play();
            cancellationTokenSource.Cancel();
        }

        private void SetCoolDownState(int currentTime, int maximumTime)
        {
            var difference = (float) currentTime / maximumTime;
            var reduceDifference = 1 - difference;
            sliderImage.fillAmount = reduceDifference;
            outerCircleImage.fillAmount = difference;

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(maximumTime - currentTime);
            timeText.text = $"{timeSpan.Minutes}:{timeSpan.Seconds:D2}";
        }

        private void DisableSliderImage()
        {
            sliderImage.gameObject.SetActive(false);
        }

        public bool SetLockState(int id)
        {
            var dictionary = SaveLoadManager.Instance.LoadSpecialAttackUpgrade();
            var state = dictionary.ContainsKey(id);

            lockBackground.SetActive(!state);
            buttonComponent.enabled = state;

            return state;
        }
    }
}