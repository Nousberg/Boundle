using Assets.Scripts.Entities.Liquids;
using System;

namespace Assets.Scripts.Saving.Data.LiquidsData
{
    [Serializable]
    public class SavedLqiuid
    {
        public LiquidContainer.LiquidType type;
        public float amount;

        public SavedLqiuid(LiquidContainer.LiquidType type, float amount)
        {
            this.type = type;
            this.amount = amount;
        }
    }
}
