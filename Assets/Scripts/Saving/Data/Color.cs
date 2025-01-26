using System;

namespace Assets.Scripts.Saving.Data
{
    [Serializable]
    public struct Color
    {
        public float r, g, b, a;

        public Color(float r = 0, float g = 0, float b = 0, float a = 0)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        public bool IsAssigned() => r != 0 && g != 0 && b != 0 && a != 0;
        public UnityEngine.Color ToUnityColor() => new UnityEngine.Color(r, g, b, a);
    }
}
