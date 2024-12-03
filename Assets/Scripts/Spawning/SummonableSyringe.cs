using Assets.Scripts.Entities.Liquids;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Spawning
{
    [RequireComponent(typeof(LiquidContainer))]
    public class SummonableSyringe : Summonable
    {
        [SerializeField] private List<Liquid> baseLiquids = new List<Liquid>();

        private void Start()
        {
            Initialize();
        }
        public override void Initialize()
        {
            LiquidContainer container = GetComponent<LiquidContainer>();

            foreach (var liquid in baseLiquids)
            {
                container.TryInject(liquid.type, liquid.amount);
            }
        }
    }
}
