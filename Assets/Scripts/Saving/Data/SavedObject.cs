using System;

namespace Assets.Scripts.Saving.Data
{
    [Serializable]
    public class SavedObject : SavedElement
    {
        public string ownerUUID;
        public SavedTransform transform;
        public SavedRigidBody rigidBody;

        public SavedObject(SavedTransform transform, SavedRigidBody rigidBody, string ownerUUID, int networkId) : base(networkId)
        {
            this.transform = transform;
            this.rigidBody = rigidBody;
            this.ownerUUID = ownerUUID;
        }
    }
}
