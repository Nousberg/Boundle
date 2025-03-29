using System;

namespace Assets.Scripts.Saving.Data
{
    [Serializable]
    public class SavedDroppedItem : SavedObject
    {
        public string jsonData;

        public SavedDroppedItem(string jsonData, string ownerUUID, int networkId, SavedTransform transform, SavedRigidBody rigidBody) : base(transform, rigidBody, ownerUUID, networkId)
        {
            this.jsonData = jsonData;
        }
    }
}
