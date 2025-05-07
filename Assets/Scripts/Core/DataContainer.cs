using Assets.Scripts.Entities;
using Assets.Scripts.Ui.Player;
using System.Collections.Generic;

namespace Assets.Scripts.Core
{
    public static class DataContainer
    {
        public static SettingsPreset settings = new SettingsPreset();
        public static bool wasBanned = false;
        public static bool wasKicked = false;

        public static Dictionary<DamageData.DamageType, DamageData> DamageProperties = new()
        {
            { DamageData.DamageType.Generic, new DamageData(true) },
            { DamageData.DamageType.Magic, new DamageData(false, 0.05f) },
            { DamageData.DamageType.Gravity, new DamageData(false) },
            { DamageData.DamageType.Kenetic, new DamageData(false) },
            { DamageData.DamageType.Temperature, new DamageData(false) }
        };
    }
}
