using System;

namespace Assets.Scripts.Saving.Data
{
    [Serializable]
    public class SavedSettings
    {
        public string playerUUID;
        public string playerName;
        public float resolution;
        public int graphicLevel;

        public SavedSettings(string playerName, string playerUUID, float resolution, int graphicLevel)
        {
            this.playerUUID = playerName;
            this.playerName = playerName;
            this.resolution = resolution;
            this.graphicLevel = graphicLevel;
        }
    }
}
