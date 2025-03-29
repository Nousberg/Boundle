using System;

namespace Assets.Scripts.Saving.Data
{
    [Serializable]
    public class SavedPhysicsProperties : SavedElement
    {
        public float temperature;

        public SavedPhysicsProperties(float temperature, int networkId) : base(networkId)
        {
            this.temperature = temperature;
        }
    }
}
