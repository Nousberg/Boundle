using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Summonables : MonoBehaviour
    {
        [SerializeField] private List<Summonable> objects = new List<Summonable>();

        public void Summon(int id, Vector3 pos, Quaternion rot)
        {
            Summonable findedObject = objects.Find(n => n.ObjectId == id);

            if (findedObject != null)
            {
                GameObject obj = Instantiate(findedObject.gameObject, pos, rot);
                obj.GetComponent<Summonable>().Initialize();
            }
        }
    }
}