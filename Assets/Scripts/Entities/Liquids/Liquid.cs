using System;

namespace Assets.Scripts.Entities.Liquids
{
    [Serializable]
    public class Liquid
    {
        public LiquidContainer.LiquidType type;
        public float amount;

        public Liquid(LiquidContainer.LiquidType type, float amount)
        {
            this.amount = amount;
            this.type = type;
        }
    }
}
