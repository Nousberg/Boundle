using System;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Entities.Liquids
{
    [RequireComponent(typeof(PhotonView))]
    public class LiquidContainer : MonoBehaviourPun
    {
        [field: SerializeField] public float Capacity { get; private set; }
        [SerializeField] private List<Liquid> liquids = new List<Liquid>();

        public List<Liquid> GetLiquids => new List<Liquid>(liquids);
        public float Weight => liquids.Sum(n => n.amount);

        public event Action OnLiquidsChanged;

        private PhotonView view => GetComponent<PhotonView>();
        private bool liquidChangeResult;

        [PunRPC]
        private void RPC_Transfer(int containerInstanceId, float amount)
        {
            if (view.IsMine)
            {
                LiquidContainer container = FindObjectsOfType<LiquidContainer>().ToList()
                    .Find(n => n.gameObject.GetInstanceID() == containerInstanceId);

                if (container != null)
                    Transfer(container, amount);
            }
        }

        [PunRPC]
        private void RPC_SetLiquidAmount(int liquidIndex, float amount)
        {
            if (liquidIndex < 0 || liquidIndex >= liquids.Count ||
                Weight - liquids[liquidIndex].amount + amount > Capacity)
                return;

            liquids[liquidIndex].amount = amount;

            if (amount <= 0f)
                liquids.RemoveAt(liquidIndex);

            OnLiquidsChanged?.Invoke();
        }

        [PunRPC]
        private void RPC_AddNewLiquid(int liquidType, float amount)
        {
            if (Weight + amount > Capacity)
                return;

            liquids.Add(new Liquid((LiquidType)liquidType, amount));
            OnLiquidsChanged?.Invoke();
        }

        [PunRPC]
        private void RPC_SendTransferResult(bool res) => liquidChangeResult = res;

        public void SetLiquidAmount(int liquidIndex, float amount)
        {
            if (!view.IsMine)
            {
                view.RPC("RPC_SetLiquidAmount", RpcTarget.All, liquidIndex, amount);
                return;
            }

            if (liquidIndex < 0 || liquidIndex >= liquids.Count ||
                Weight - liquids[liquidIndex].amount + amount > Capacity)
                return;

            liquids[liquidIndex].amount = amount;

            if (amount <= 0f)
                liquids.RemoveAt(liquidIndex);

            OnLiquidsChanged?.Invoke();
        }

        public void Transfer(LiquidContainer target, float amount)
        {
            if (!view.IsMine)
            {
                view.RPC("RPC_Transfer", RpcTarget.All, target.gameObject.GetInstanceID(), amount);
                return;
            }

            if (amount <= 0f || amount > Weight)
                return;

            List<Liquid> liquidsToProcess = new List<Liquid>(liquids);
            foreach (var liquid in liquidsToProcess)
            {
                float transferAmount = Mathf.Clamp(amount, 0f, liquid.amount);
                int liquidIndex = liquids.IndexOf(liquid);

                target.Inject(liquid.type, transferAmount, view);
                SetLiquidAmount(liquidIndex, liquid.amount - transferAmount);
            }
        }

        public void Inject(LiquidType liquidType, float amount, PhotonView initiator)
        {
            if (amount <= 0f || Weight + amount > Capacity)
            {
                initiator.RPC("RPC_SendTransferResult", RpcTarget.All, false);
                return;
            }

            int existingIndex = liquids.FindIndex(n => n.type == liquidType);
            if (existingIndex != -1)
            {
                float newAmount = liquids[existingIndex].amount + amount;
                if (Weight - liquids[existingIndex].amount + newAmount > Capacity)
                {
                    initiator.RPC("RPC_SendTransferResult", RpcTarget.All, false);
                    return;
                }

                SetLiquidAmount(existingIndex, newAmount);
            }
            else
            {
                if (view.IsMine)
                {
                    liquids.Add(new Liquid(liquidType, amount));
                    OnLiquidsChanged?.Invoke();
                }
                else
                {
                    view.RPC("RPC_AddNewLiquid", RpcTarget.All, (int)liquidType, amount);
                }
            }

            initiator.RPC("RPC_SendTransferResult", RpcTarget.All, true);
        }

        public void Pumpout(LiquidType liquidType, float amount)
        {
            int liquidIndex = liquids.FindIndex(n => n.type == liquidType);
            if (liquidIndex == -1 || liquids[liquidIndex].amount < amount || amount <= 0f)
                return;

            SetLiquidAmount(liquidIndex, liquids[liquidIndex].amount - amount);
        }

        public enum LiquidType : byte
        {
            Water,
            Acid,
            Blood,
            Mending
        }
    }
}