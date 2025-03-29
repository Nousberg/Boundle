using System;

namespace Assets.Scripts.Saving.Data
{
    [Serializable]
    public class Color
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public Color(float r = float.NegativeInfinity, float g = float.NegativeInfinity, float b = float.NegativeInfinity, float a = float.NegativeInfinity)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        public bool IsAssigned() => r != float.NegativeInfinity && g != float.NegativeInfinity && b != float.NegativeInfinity && a != float.NegativeInfinity;
        public UnityEngine.Color ToUnityColor() => new UnityEngine.Color(r, g, b, a);
    }
}
