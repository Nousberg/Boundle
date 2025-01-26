using Assets.Scripts.Core.Environment;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Spawning
{
    [RequireComponent(typeof(SceneData))]
    public class Summonables : MonoBehaviour
    {
        [field: SerializeField] public List<Summonable> SummonableObjects { get; private set; } = new List<Summonable>();
        public event Action<GameObject> OnSummoned;
        public event Action<int> OnDestroyed;
        public List<GameObject> SummonedObjects { get; private set; } = new List<GameObject>();

        private SceneData sceneData => GetComponent<SceneData>();

        public void Init() => StartCoroutine(SpawnPlayer());

        private IEnumerator SpawnPlayer()
        {
            yield return new WaitUntil(() => PhotonNetwork.InRoom);

            Summon(12, sceneData.PlayersSpawnPosition, sceneData.PlayersSpawnRotation);
        }

        public GameObject Summon(int id, Vector3 pos, Quaternion rot, Transform parent = null)
        {
            Summonable findedObject = SummonableObjects.Find(n => n.ObjectId == id);
            if (findedObject != null)
            {
                GameObject obj = PhotonNetwork.Instantiate(findedObject.name, pos, rot);
                obj.transform.parent = parent;
                Summonable data = obj.GetComponent<Summonable>();
                data.Initialize(gameObject);
                SummonedObjects.Add(obj);
                OnSummoned?.Invoke(obj);
                StartCoroutine(HandleObjDestroy(obj));
                return obj;
            }

            return null;
        }
        private IEnumerator HandleObjDestroy(GameObject obj)
        {
            int objectId = obj.GetInstanceID();
            yield return new WaitUntil(() => obj == null);
            OnDestroyed?.Invoke(objectId);
            SummonedObjects.Remove(obj);
        }
        public enum ObjectCategory : byte
        {
            None,
            Living,
            Rifle,
            Syringe,
            Prop
        }
    }
}