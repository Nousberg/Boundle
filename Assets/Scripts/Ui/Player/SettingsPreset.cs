namespace Assets.Scripts.Ui.Player
{
    public class SettingsPreset
    {
        public float masterVol;
        public float environmentVol;
        public float musicVol;
        public float sfxVol;

        public int resolution;
        public int quality;

        public SettingsPreset(int resolution = 80, int quality = 1, float masterVol = 0f, float musicVol = -20f, float sfxVol = -20f, float environmentVol = -20f)
        {
            this.resolution = resolution;
            this.quality = quality;

            this.masterVol = masterVol;
            this.musicVol = musicVol;
            this.sfxVol = sfxVol;
            this.environmentVol = environmentVol;
        }
    }
}
