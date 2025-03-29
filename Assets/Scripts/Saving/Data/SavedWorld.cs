using System;
using System.Collections.Generic;

namespace Assets.Scripts.Saving.Data
{
    [Serializable]
    public class SavedWorld
    {
        public List<SavedEntity> entities = new List<SavedEntity>();
        public List<SavedObject> objects = new List<SavedObject>();
        public List<SavedDroppedItem> droppedItems = new List<SavedDroppedItem>();
    }
}
