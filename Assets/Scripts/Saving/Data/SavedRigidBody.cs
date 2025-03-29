using System;

namespace Assets.Scripts.Saving.Data
{
    [Serializable]
    public class SavedRigidBody : SavedElement
    {
        public bool kinematic;
        public bool useGravity;
        public SavedVector3 velocity;

        public SavedRigidBody(SavedVector3 velocity, bool kinematic, bool useGravity, int networkId) : base(networkId)
        {
            this.velocity = velocity;
            this.kinematic = kinematic;
            this.useGravity = useGravity;
        }
    }
}
