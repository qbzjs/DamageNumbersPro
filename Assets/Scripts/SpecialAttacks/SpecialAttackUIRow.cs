using System.Collections.Generic;
using ScriptableObjects;
using UI;
using UnityEngine;
using Utils;

namespace SpecialAttacks
{
    public class SpecialAttackUIRow : UIRow
    {
        [SerializeField]
        private SkillIconDataSO skillIconDataSo;

        private SpecialAttackUpgrade _specialAttackUpgrade;

        private int _level = 1;

        private Dictionary<int, int> _specialAttackDictionary;

        private Dictionary<int, int> _saveData;

        private void Start()
        {
            UIManager.OnUpdateCoinHud += UpdateRow;
        }

        public override void SetUIRow(UpgradableStat upgradableStat)
        {
            _specialAttackUpgrade = (SpecialAttackUpgrade) upgradableStat;
            cellIdentifier = _specialAttackUpgrade.ID.ToString();

            var coin = SaveLoadManager.Instance.LoadCoin();

            UpdateRow(coin);
        }

        public override void FillUIRow()
        {
            _specialAttackDictionary = SaveLoadManager.Instance.LoadSpecialAttackUpgrade();

            if (_specialAttackDictionary.ContainsKey(_specialAttackUpgrade.ID))
            {
                _level = _specialAttackDictionary[_specialAttackUpgrade.ID];
            }
            else
            {
                _level = 1;
            }

            var damage = CalcUtils.FormatNumber(_specialAttackUpgrade.BaseIncrementAmount * _level);

            var stringBuilder = DescriptionUtils.GetDescription(_specialAttackUpgrade.SkillTypes);
            if (stringBuilder.ToString().Contains("x"))
            {
                stringBuilder.Replace("x", damage);
            }

            descriptionText.text = stringBuilder.ToString();
            levelText.text = $"Level {_level}";

            icon.sprite = skillIconDataSo.GetIcon(_specialAttackUpgrade.ID);
        }

        public override void SetButtonState(double totalCoin)
        {
            var cost = _specialAttackUpgrade.BaseIncrementCost * _level;
            buttonCostText.text = $"{CalcUtils.FormatNumber(cost)} <sprite index= 11>";

            buttonDescriptionText.text = _level > 1 ? "LEVEL UP" : "BUY";

            buyButton.enabled = cost <= totalCoin;
            buyButtonImage.sprite = buyButton.enabled ? activeButtonSprite : deActiveButtonSprite;
        }

        public override void OnBuy()
        {
            var coin = SaveLoadManager.Instance.LoadCoin();
            var cost = _specialAttackUpgrade.BaseIncrementCost * _level;
            if (coin >= cost)
            {
                _level++;
                SaveLoadManager.Instance.SaveSkillUpgrade(_specialAttackUpgrade.ID, _level);
                Calculator.OnUpdateDamageCalculation.Invoke(_specialAttackUpgrade.ID, _level);
                EconomyManager.OnSpendCoin.Invoke(-cost);
                coin -= cost;

                UpdateRow(coin);
            }
        }

        public override void UpdateRow(double totalCoin)
        {
            FillUIRow();
            SetButtonState(totalCoin);
        }
    }
}