using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using Template.Data;
using Mobiray.Common;

namespace Template.Configs
{
    [CreateAssetMenu(fileName = "ConfigMain", menuName = "Configs/ConfigMain")]
    public class ConfigMain : ScriptableObject, INeedInitialization
    {
        public long StartMoney;

        [Space]
        public TextAsset MoneyOnLevelFile;
        public TextAsset MoneyToLevelUpFile;

        [Space]
        public float LevelUpBonusPercent;
        public List<long> MoneyOnLevel;
        public List<long> MoneyToLevelUp;

        [Space] 
        public long BasePriceParameter1;
        public float PriceStepParameter1;
        
        [Space]
        public float BaseParameter1Val;
        public float StepParameter1Val;
        
        public void Initialize()
        {
            //READ CONFIGS
            ReadConfigMoneyOnLevel();
            ReadConfigMoneyToLevelUp();
        }

        public DataGameState InitState()
        {
            var dataGame = new DataGameState();

            dataGame.Money = StartMoney;

            dataGame.TotalMoney = dataGame.Money;

            return dataGame;
        }

        public int GetParameter1Val(int level)
        {
            //BaseParameter1Val
            //StepParameter1Val
            return level + 1;
        }

        public long GetParameter1Price(int level)
        {
            return (long) (BasePriceParameter1 * (1 + PriceStepParameter1 * level));
        }

        private void ReadConfigMoneyOnLevel()
        {
            MoneyOnLevel = new List<long>();
            var moneyOnLevelText = MoneyOnLevelFile.text;

            var provider = new CultureInfo("en-US");
            var styles = NumberStyles.Integer | NumberStyles.AllowDecimalPoint;

            var lines = moneyOnLevelText.Split('\n');


            for (int i = 1; i < lines.Length; i++)
            {
                var args = lines[i].Split(',');

                MoneyOnLevel.Add(long.Parse(args[1]));
            }
        }
        
        private void ReadConfigMoneyToLevelUp()
        {
            MoneyToLevelUp = new List<long>();
            var moneyOnLevelText = MoneyToLevelUpFile.text;

            var provider = new CultureInfo("en-US");
            var styles = NumberStyles.Integer | NumberStyles.AllowDecimalPoint;

            var lines = moneyOnLevelText.Split('\n');


            for (int i = 1; i < lines.Length; i++)
            {
                MoneyToLevelUp.Add(long.Parse(lines[i]));
            }
        }
    }
}