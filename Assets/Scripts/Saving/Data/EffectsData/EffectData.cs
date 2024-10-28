using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Saving
{
    public class EffectData
    {
        public float duration;
        public float amplifier;
        public string type;

        public EffectData(float duration, float amplifier, string type)
        {
            this.duration = duration;
            this.amplifier = amplifier;
            this.type = type;
        }
    }
}
