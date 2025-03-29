using System;

namespace Assets.Scripts.Saving.Data
{
    [Serializable]
    public class SavedElement
    {
        public int networkId;

        public SavedElement(int networkId)
        {
            this.networkId = networkId;
        }
    }
}
