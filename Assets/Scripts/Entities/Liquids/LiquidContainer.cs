using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Entities.Liquids
{
    public class LiquidContainer : MonoBehaviour
    {
        [field: SerializeField] public float Capacity { get; private set; }
        [SerializeField] private List<Liquid> liquids = new List<Liquid>();

        public List<Liquid> GetLiquids => liquids;
        public float Weight => liquids.Sum(n => n.amount);

        public event Action OnLiquidsChanged;

        public void Transfer(LiquidContainer target, float amount)
        {
            if (amount <= 0f || amount > Weight)
                return;

            List<Liquid> liquidsToRemove = new List<Liquid>();

            foreach (var liquid in liquids)
            {
                if (!target.TryInject(liquid.type, Mathf.Clamp(amount, 0f, liquid.amount)))
                    return;

                liquid.amount -= Mathf.Clamp(amount, 0f, liquid.amount);

                if (liquid.amount <= 0f)
                    liquidsToRemove.Add(liquid);
            }

            foreach (var liquid in liquidsToRemove)
                liquids.Remove(liquid);

            OnLiquidsChanged?.Invoke();
        }
        public bool TryInject(LiquidType liquid, float amount)
        {
            if (amount <= 0f || Weight + amount > Capacity)
                return false;

            Liquid findedLiquid = liquids.Find(n => n.type == liquid);

            if (findedLiquid != null)
            {
                if (Weight + findedLiquid.amount > Capacity)
                    return false;

                findedLiquid.amount += amount;
            }
            else
                liquids.Add(new Liquid(liquid, amount));

            OnLiquidsChanged?.Invoke();
            return true;
        }
        public bool TryPumpout(LiquidType liquid, float amount)
        {
            Liquid findedLiquid = liquids.Find(n => n.type == liquid);

            if (findedLiquid == null || findedLiquid.amount < amount || amount <= 0f)
                return false;

            findedLiquid.amount -= amount;

            if (findedLiquid.amount <= 0f)
                liquids.Remove(findedLiquid);

            OnLiquidsChanged?.Invoke();
            return true;
        }
        public bool TrySetLiquidAmount(int liquidIndex, float amount)
        {
            if (liquidIndex < 0 || liquidIndex >= liquids.Count || Weight - liquids[liquidIndex].amount + amount > Capacity)
                return false;

            liquids[liquidIndex].amount = amount;

            if (amount <= 0f)
                liquids.RemoveAt(liquidIndex);

            OnLiquidsChanged?.Invoke();
            return true;
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