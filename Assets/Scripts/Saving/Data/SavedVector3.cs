using System;

namespace Assets.Scripts.Saving.Data
{
    [Serializable]
    public class SavedVector3 : SavedElement
    {
        public float X;
        public float Y;
        public float Z;

        public SavedVector3(float x, float y, float z, int networkId) : base(networkId)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
