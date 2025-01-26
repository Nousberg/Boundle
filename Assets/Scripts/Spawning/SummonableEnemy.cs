using Assets.Scripts.Entities;
using UnityEngine;

namespace Assets.Scripts.Spawning
{
    [RequireComponent(typeof(Entity))]
    public class SummonableEnemy : Summonable
    {
        Entity e => GetComponent<Entity>();

        public override void Initialize(GameObject metaObject) => e.Init();
    }
}
