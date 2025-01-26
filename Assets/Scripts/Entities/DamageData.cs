namespace Assets.Scripts.Entities
{
    public class DamageData
    {
        public readonly bool IgnoreDefence;
        public readonly float InvulnerabilityMitigation;

        public DamageData(bool ignoreDefence, float invulnerabilityMitigation = 0f)
        {
            IgnoreDefence = ignoreDefence;
            InvulnerabilityMitigation = invulnerabilityMitigation;
        }

        public enum DamageType : byte
        {
            Generic,
            Gravity,
            Kenetic,
            Magic,
            Temperature
        }
    }
}
