using Assets.Scripts.Effects;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Effects
{
    public class DynamicEffectUiData
    {
        public Image icon;
        public Effect effect;

        public DynamicEffectUiData(Image icon, Effect index)
        {
            this.icon = icon;
            this.effect = index;
        }
    }
}
