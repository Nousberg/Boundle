using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Abilities;
using Assets.Scripts.Inventory;
using Assets.Scripts.Inventory.View;
using Assets.Scripts.Movement;
using Assets.Scripts.Network;
using Assets.Scripts.Network.Chat;
using Assets.Scripts.Ui.Chat;
using Assets.Scripts.Ui.Effects;
using Assets.Scripts.Ui.Inventory;
using Assets.Scripts.Ui.Player;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Spawning
{
    [RequireComponent(typeof(PhotonView))]
    public class SummonablePlayer : Summonable
    {
        [SerializeField] private InputInitializer inputInitializer;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private UiChatController chatController;
        [SerializeField] private TextMeshProUGUI plrName;
        [SerializeField] private TextMeshProUGUI serverName;
        [SerializeField] private InventoryDataVisualizer invVisual;
        [SerializeField] private PlayerEffectsVisualizer effects;
        [SerializeField] private ChatInteractProvider chat;
        [SerializeField] private GameVisualManager visualManager;
        [SerializeField] private GameObject uiCanvas;
        [SerializeField] private GameObject cam;
        [SerializeField] private ItemSway sway;
        [SerializeField] private Entity playerEntity;
        [SerializeField] private PlayerInventoryController invController;
        [SerializeField] private InventoryDataController inventory;
        [SerializeField] private FlyAbility fly;
        [SerializeField] private PlayerMovementLogic movement;
        [SerializeField] private PlayerCameraLogic camMovement;
        [SerializeField] private ToolgunDataController toolgun;

        private PhotonView View => GetComponent<PhotonView>();

        public override void Initialize(GameObject metaObject)
        {
            if (View.IsMine)
            {
                rb.useGravity = true;

                plrName.text = PhotonNetwork.NickName;
                serverName.text = PhotonNetwork.CurrentRoom.Name;

                effects.Init();

                if (metaObject.TryGetComponent<InputMachine>(out var inputMachine))
                {
                    inputInitializer.Init(inputMachine);
                    visualManager.Init(inputMachine);

                    if (metaObject.TryGetComponent<Summonables>(out var summonables))
                        toolgun.Init(summonables, inputMachine);
                }

                if (metaObject.TryGetComponent<ChatInputHandler>(out var chatInput))
                {
                    if (metaObject.TryGetComponent<CommandParser>(out var p))
                        chatController.Init(chatInput, p);

                    chat.Init(chatInput);
                }

                uiCanvas.SetActive(true);
                cam.SetActive(true);

                sway.Init();
                invController.Init();
                movement.Init();
                camMovement.Init();

                playerEntity.Init();
                fly.Init();
                invVisual.Init();
                inventory.Init();
            }
        }
    }
}
