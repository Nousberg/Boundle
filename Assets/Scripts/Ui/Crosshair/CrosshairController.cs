using Assets.Scripts.Entities;
using Assets.Scripts.Inventory;
using Assets.Scripts.Movement;
using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Inventory.Scriptables;

namespace Assets.Scripts.Ui.Crosshair
{
    public class CrosshairController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<CrosshairDataContainer> crosshairs = new List<CrosshairDataContainer>();

        [Header("Properties")]
        [SerializeField] private float velocityMultiplier;
        [SerializeField] private float minHealthOpacityInfluence;
        [SerializeField] private float updateSpeed;
        [SerializeField] private float minVelocitySizeInfluence;
        [SerializeField] private float minVelocitySpacingInfluence;

        [Header("Switching")]
        [Range(0f, 1f)][SerializeField] private float minVisibility;
        [SerializeField] private float crosshairSwitchingDuration;
        [SerializeField] private Ease crosshairSwitchingEase;

        private CrosshairDataContainer currentCrosshair;
        private PlayerMovementLogic movement;
        private InventoryDataController inventory;
        private Entity player;
        private PhotonView view;

        private Vector2 spacingOffset;
        private Vector2 sizeOffset;
        private float opacityOffset;
        private float recoilOffset;
        private bool initialized;
        private bool allowUpdate = true;
        private float currentUpdateSpeed;

        public void Init(PlayerMovementLogic movement, InventoryDataController inventory, Entity player)
        {
            initialized = true;
            currentUpdateSpeed = updateSpeed;

            this.inventory = inventory;
            this.movement = movement;
            this.player = player;

            inventory.OnItemSwitched += ChangeCurrentCrosshairByItem;
            inventory.OnItemAdded += (n) => LinkCrosshairToItems();
            inventory.OnItemRemoved += (n) => LinkCrosshairToItems();

            LinkCrosshairToItems();
            ChangeCurrentCrosshairByItem();
        }

        private void LinkCrosshairToItems()
        {
            foreach (var item in inventory.GetItems)
            {
                if (inventory.AllInGameItems.Find(n => n.BaseData.Id == item.data.Id) is WeaponDataController weapon)
                {
                    BaseWeaponData wData = weapon.BaseData as BaseWeaponData;
                    weapon.OnFire -= () => recoilOffset = wData.Recoil;
                    weapon.OnFire += () => recoilOffset = wData.Recoil;
                }
            }
        }
        private void ChangeCurrentCrosshairByItem()
        {

        }
        private void Update()
        {
            if (initialized && allowUpdate && !currentCrosshair.immutableOpacity)
            {
                float velocityInfluence = Mathf.Max(movement.CurrentVelocity * velocityMultiplier, 1f);

                float targetCrosshairTransperency = Mathf.Lerp(currentCrosshair.opacity.alpha, movement.CurrentVelocity > 0.25f ? currentCrosshair.defaultOpacity *
                Mathf.Max(player.Health / player.BaseHealth, minHealthOpacityInfluence) / velocityInfluence : 0f,
                currentUpdateSpeed * Time.deltaTime);

                currentCrosshair.opacity.alpha = targetCrosshairTransperency;
            }
        }

        [Serializable]
        private class CrosshairDataContainer
        {
            public bool immutableOpacity;
            public bool immutableSpacing;
            [Range(0f, 1f)] public float defaultOpacity;
            public Vector2 defaultSpacing;
            public Crosshair variant;
            public GameObject holder;
            public CanvasGroup opacity;
            public GridLayoutGroup size;
        }

        public enum Crosshair
        {
            Full,
            Dot,
            None,
            Unassigned
        }
    }
}
