using Assets.Scripts.Saving.Data;
using System;

namespace Assets.Scripts.Saving
{
    [Serializable]
    public class SavedGame
    {
        public SavedSettings settings;

        public SavedGame(SavedSettings settings)
        {
            this.settings = settings;
        }
    }
}
