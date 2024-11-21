using System;

namespace Assets.Scripts.Achivements
{
    [Serializable]
    public class Achivement
    {
        public string name;

        public Achivement(string name)
        {
            this.name = name;
        }
    }
}