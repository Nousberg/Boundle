using Assets.Scripts.Entities;
using Assets.Scripts.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Achivements
{
    [RequireComponent(typeof(Entity))]
    public class AchivementProvider : MonoBehaviour
    {
        private List<Achivement> Achivements = new List<Achivement>();
        private Entity player => GetComponent<Entity>();

        private void Start()
        {
            Achivements = JsonSaver.Load<List<Achivement>>("Achivements/", "Achivements");
            player.OnDeath += DeathAchivement;
        }
        private void OnApplicationQuit()
        {
            JsonSaver.Save(Achivements, "Achivements/", "Achivements");
        }
        private void DeathAchivement()
        {
            if (Achivements.Find(n => n.name == "FirstDeath") == null)
                Achivements.Add(new Achivement("FirstDeath"));
        }
        public List<Achivement> GetAchivements()
        {
            return Achivements;
        }
    }
}