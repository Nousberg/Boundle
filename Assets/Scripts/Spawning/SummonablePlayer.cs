using Assets.Scripts.Core;
using Assets.Scripts.Core.Environment;
using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.Sound;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Abilities;
using Assets.Scripts.Inventory;
using Assets.Scripts.Inventory.View;
using Assets.Scripts.Movement;
using Assets.Scripts.Ui.Inventory;
using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts.Spawning
{
    [RequireComponent(typeof(PhotonView))]
    public class SummonablePlayer : Summonable
    {
        [SerializeField] private GameObject playerModel;
        [SerializeField] private PlayerSoundController soundController;
        [SerializeField] private PlayerRespawnProvider respawner;
        [SerializeField] private GameObject nonLocalCanvas;
        [SerializeField] private MeleeDataController melee;
        [SerializeField] private ShotgunDataController shotgun;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private ItemSway sway;
        [SerializeField] private Entity playerEntity;
        [SerializeField] private PlayerInventoryController invController;
        [SerializeField] private InventoryDataController inventory;
        [SerializeField] private FlyAbility fly;
        [SerializeField] private PlayerMovementLogic movement;
        [SerializeField] private PlayerCameraLogic camMovement;

        public GameObject cam;
        public ToolgunDataController toolgun;

        private PhotonView View => GetComponent<PhotonView>();

        public override void Initialize(GameObject metaObject)
        {
            if (View.IsMine)
            {
                playerModel.SetActive(false);
                nonLocalCanvas.SetActive(false);

                if (metaObject.TryGetComponent<Bootstrap>(out var boot))
                {
                    rb.useGravity = true;

                    if (metaObject.TryGetComponent<InputMachine>(out var inputMachine))
                    {
                        InitInput(metaObject, inputMachine, boot);

                        if (metaObject.TryGetComponent<SceneData>(out var sceneData))
                            respawner.Init(inputMachine, sceneData);
                    }

                    cam.SetActive(true);

                    if (metaObject.TryGetComponent<SoundManager>(out var soundManager))
                    {
                        soundController.Init(soundManager);
                    }
                    playerEntity.Init();
                    inventory.Init();
                }
            }
        }
        private void InitInput(GameObject metaObject, InputMachine machine, Bootstrap boot)
        {
            MovementInputState movementState = new MovementInputState();
            ToolgunInputState toolgunState = new ToolgunInputState();
            InventoryControllerInputState inventoryState = new InventoryControllerInputState();
            AbilityInputState abilityState = new AbilityInputState();
            WeaponInputState weaponState = new WeaponInputState();

            shotgun.Init(weaponState);
            melee.Init(weaponState);
            fly.Init(abilityState);
            sway.Init(movementState);
            invController.Init(inventoryState);
            movement.Init(movementState);
            camMovement.Init(movementState);

            if (metaObject.TryGetComponent<Summonables>(out var summonables))
                toolgun.Init(summonables, machine, toolgunState);

            machine.AddState(weaponState);
            machine.AddState(abilityState);
            machine.AddState(inventoryState);
            machine.AddState(toolgunState);
            machine.AddState(movementState);
            machine.Init();
        }
    }
}
