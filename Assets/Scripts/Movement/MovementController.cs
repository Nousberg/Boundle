﻿using Assets.Scripts.Movement.Scriptables;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public abstract class MovementController : MonoBehaviour
    {
        [field: SerializeField] protected MovementData data { get; private set; }

        public delegate void MovementHandler(MovementData data, ref float speed, ref float flySpeed, ref float runSpeedBoost, ref float jumpPower);

        public event MovementHandler OnMove;

        protected float flySpeed;
        protected float runSpeedBoost;
        protected float jumpPower;
        protected bool isCrouch;
        protected float currentWalkSpeed;

        protected void Move()
        {
            OnMove?.Invoke(data, ref currentWalkSpeed, ref flySpeed, ref runSpeedBoost, ref jumpPower);
        }
    }
}