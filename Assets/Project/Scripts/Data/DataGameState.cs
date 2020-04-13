using System;
using System.Collections.Generic;
using Mobiray.Common;
using Template.Configs;
using Template.Controllers;
using UnityEngine;

namespace Template.Data
{
    [Serializable]
    public class DataGameState
    {
        public long Money;
        public int HardCurrency;
        public long TotalMoney;

        public int LastLevelUp;
        public int CurrentMoneyLevel;

        public int CurrentLevel;

        [Space]
        public int LevelParamter1;

        public DateTime AppClosedDateTime;

        public void AddMoney(long value)
        {
            Money += value;
            // if (value > 0) TotalMoney += value;
            ToolBox.Signals.Send(new SignalAddMoney {Money = value});
        }

        public void AddHard(int value)
        {
            HardCurrency += value;
            ToolBox.Signals.Send(new SignalAddHardCurrency {HardCurrency = value});
        }

        public bool BuySoft(long price)
        {
            if (Money < price)
            {
                Debug.LogError("Not enough money - cur: " + Money + " price: " + price); //TODO add money window?
                return false;
            }

            Money -= price;
            return true;
        }

        public bool BuyHard(int price)
        {
            if (HardCurrency < price)
            {
                Debug.LogError("Not enough Hard Currency"); //TODO add money window?
                return false;
            }

            HardCurrency -= price;

            ToolBox.Signals.Send(new SignalAddHardCurrency {HardCurrency = -price});
            return true;
        }

        public void BuyParameter1Upgrade()
        {
            var config = ToolBox.Get<ConfigMain>();
            var price = config.GetParameter1Price(LevelParamter1);
            
            if (BuySoft(price))
            {
                LevelParamter1++;
                
                ToolBox.Signals.Send(new SignalUpgradeCharacter());
                ToolBox.Signals.Send(new SignalAddMoney {Money = -price});
                
                ToolBox.Get<SoundController>().PlayUi();
                
                Analytics.OnEvent("buy_parameter", new Dictionary<string, object>
                {
                    {"level", LevelParamter1}
                });
            }
        }

    }
}