using System;
using System.Collections.Generic;

namespace Assets.Scripts.Saving.Data.InventoryData
{
    [Serializable]
    public class SavedItem : SavedElement
    {
        public Dictionary<string, int> intAttributes = new Dictionary<string, int>();
        public Dictionary<string, float> floatAttributes = new Dictionary<string, float>();
        public int id;

        public SavedItem(int id, Dictionary<string, float> floats, Dictionary<string, int> ints, int networkId) : base(networkId)
        {
            this.id = id;
            intAttributes = ints;
            floatAttributes = floats;
        }
    }
}