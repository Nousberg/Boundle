using Assets.Scripts.Entities.Liquids;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Spawning
{
    [RequireComponent(typeof(LiquidContainer))]
    [RequireComponent(typeof(PhotonView))]
    public class SummonableSyringe : Summonable
    {
        [Header("References")]
        [SerializeField] private Syringe syringe;

        [Header("Properties")]
        [SerializeField] private List<Liquid> baseLiquids = new List<Liquid>();

        private PhotonView view => GetComponent<PhotonView>();

        public override void Initialize(GameObject metaObject)
        {
            LiquidContainer container = GetComponent<LiquidContainer>();

            foreach (var liquid in baseLiquids)
                container.Inject(liquid.type, liquid.amount, view);

            syringe.Init();
        }
    }
}
