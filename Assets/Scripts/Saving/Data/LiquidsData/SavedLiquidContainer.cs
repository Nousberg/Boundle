using System;
using System.Collections.Generic;

namespace Assets.Scripts.Saving.Data.LiquidsData
{
    [Serializable]
    public class SavedLiquidContainer
    {
        public List<SavedLqiuid> Lqiuids = new List<SavedLqiuid>();

        public SavedLiquidContainer(List<SavedLqiuid> lqiuids)
        {
            Lqiuids = lqiuids;
        }
    }
}
