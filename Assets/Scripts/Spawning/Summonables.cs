using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Spawning
{
    public class Summonables : MonoBehaviour
    {
        [field: SerializeField] public List<Summonable> objects { get; private set; } = new List<Summonable>();

        public GameObject Summon(int id, Vector3 pos, Quaternion rot, Transform parent)
        {
            Summonable findedObject = objects.Find(n => n.ObjectId == id);

            if (findedObject != null)
            {
                GameObject obj = Instantiate(findedObject.gameObject, pos, rot);
                obj.transform.parent = parent;

                Summonable data = obj.GetComponent<Summonable>();
                data.Initialize();
                Destroy(data);

                return obj;
            }

            return null;
        }

        public enum ObjectCategory : byte
        {
            Rifle,
            Syringe,
            Prop
        }
    }
}