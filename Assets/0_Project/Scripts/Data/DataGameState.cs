using System;
using System.Collections.Generic;
using Mobiray.Common;
using CriminalTown.Configs;
using CriminalTown.Controllers;
using UnityEngine;

namespace CriminalTown.Data
{
    [Serializable]
    public class DataGameState
    {
        public int money;

        public List<DataIsland> islands;

        [Space]
        // public int LevelParamter1;
        public DateTime appClosedDateTime;

        public void AddMoney(int value)
        {
            money += value;
            // if (value > 0) TotalMoney += value;
            ToolBox.Signals.Send(new SignalAddMoney {money = value});
        }

        /*public void AddHard(int value)
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
        }*/

    }
}