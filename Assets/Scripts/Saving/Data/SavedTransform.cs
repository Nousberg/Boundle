using System;

namespace Assets.Scripts.Saving.Data
{
    [Serializable]
    public class SavedTransform : SavedElement
    {
        public SavedVector3 position;
        public SavedVector3 rotation;
        public SavedVector3 scale;

        public SavedTransform(SavedVector3 position, SavedVector3 rotation, SavedVector3 scale, int networkId) : base(networkId)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
}
