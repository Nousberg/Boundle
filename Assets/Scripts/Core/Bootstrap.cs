using Assets.Scripts.Core.Environment;
using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.InputSystem;
using Assets.Scripts.Effects;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Abilities;
using Assets.Scripts.Inventory;
using Assets.Scripts.Movement;
using Assets.Scripts.Network;
using Assets.Scripts.Network.Chat;
using Assets.Scripts.Spawning;
using Assets.Scripts.Ui;
using Assets.Scripts.Ui.Chat;
using Assets.Scripts.Ui.Core;
using Assets.Scripts.Ui.Crosshair;
using Assets.Scripts.Ui.Effects;
using Assets.Scripts.Ui.Multiplayer;
using Assets.Scripts.Ui.Player;
using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Core
{
    public class Bootstrap : MonoBehaviour
    {
        [field: SerializeField] public Image AmmoBackground { get; private set; }
        [field: SerializeField] public TextMeshProUGUI AmmoText { get; private set; }
        [field: SerializeField] public GameObject AmmoContainer { get; private set; }
        [field: SerializeField] public Transform ItemIconsParent { get; private set; }
        [field: SerializeField] public Transform EffectsParent { get; private set; }

        [SerializeField] private WindowController windows;
        [SerializeField] private CrosshairController crosshair;
        [SerializeField] private Button settingsMenu;
        [SerializeField] private Button playerMenu;
        [SerializeField] private Button serverMenu;
        [SerializeField] private Button tabPlayers;
        [SerializeField] private Button tabGraphics;
        [SerializeField] private Button tabAudio;
        [SerializeField] private InputHandler input;
        [SerializeField] private InputMachine inputMachine;
        [SerializeField] private PlayerEffectsVisualizer effects;
        [SerializeField] private GameVisualManager visual;
        [SerializeField] private Button leave;
        [SerializeField] private Button respawn;
        [SerializeField] private Toggle noclip;
        [SerializeField] private Toggle invulnerability;
        [SerializeField] private TextMeshProUGUI serverName;
        [SerializeField] private UiChatController chatUi;
        [SerializeField] private ChatInputHandler chat;
        [SerializeField] private CommandParser cmd;
        [SerializeField] private PlayerValidator playerValidator;
        [SerializeField] private Summonables summoner;
        [SerializeField] private SceneData sceneData;

        private IEnumerator Start()
        {
            input.Init();
            inputMachine.Init();

            serverMenu.onClick.AddListener(() =>
            {
                windows.Open(8, 0);
                windows.Open(1, 0);
                windows.Open(0, 0);
            });
            playerMenu.onClick.AddListener(() =>
            {
                windows.Open(0, 1);
                windows.Open(3, 1);
            });
            settingsMenu.onClick.AddListener(() =>
            {
                windows.Open(7, 2);
                windows.Open(2, 2);
                windows.Open(0, 2);
            });

            tabPlayers.onClick.AddListener(() => windows.Open(9, 3));
            tabGraphics.onClick.AddListener(() => windows.Open(4, 4));
            tabAudio.onClick.AddListener(() => windows.Open(5, 5));

            yield return new WaitUntil(() => PhotonNetwork.InRoom);

            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

            if (string.IsNullOrEmpty(PlayerPrefs.GetString(Connector.PLAYER_ID_KEY)))
                PlayerPrefs.SetString(Connector.PLAYER_ID_KEY, Guid.NewGuid().ToString());

            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(Connector.PLAYER_STATUS_KEY))
                playerProperties.Add(Connector.PLAYER_STATUS_KEY, -1);

            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(Connector.PLAYER_ID_KEY))
                playerProperties.Add(Connector.PLAYER_ID_KEY, PlayerPrefs.GetString(Connector.PLAYER_ID_KEY));

            if (PlayerPrefs.GetInt(Connector.ADMIN_KEY) == 1)
                playerProperties[Connector.PLAYER_STATUS_KEY] = 1;

            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

            DOTween.SetTweensCapacity(short.MaxValue, short.MaxValue);
            
            GameObject player = summoner.Summon(12, sceneData.PlayersSpawnPosition, Vector3.one, sceneData.PlayersSpawnRotation);
            
            InventoryDataController plrInventory = player.GetComponent<InventoryDataController>();
            PlayerMovementLogic movement = player.GetComponent<PlayerMovementLogic>();
            Entity plrEntity = player.GetComponent<Entity>();

            EntityNetworkData eData = player.GetComponent<EntityNetworkData>();
            playerValidator.Init(eData);
            chatUi.Init(chat, cmd);
            chat.Init(player.GetComponent<PhotonView>(), eData);
            cmd.Init();
            
            serverName.text = PhotonNetwork.CurrentRoom.Name;
            invulnerability.onValueChanged.AddListener((v) => plrEntity.Invulnerable = v);
            respawn.onClick.AddListener(() => plrEntity.Kill());

            effects.Init(player.GetComponent<EffectContainer>());
            
            FlyAbility plrFly = player.GetComponent<FlyAbility>();
            noclip.onValueChanged.AddListener((v) => plrFly.ToggleUsability(v));

            SummonablePlayer plr = player.GetComponent<SummonablePlayer>();
            visual.Init(plr.cam, plrEntity, plrFly, movement, plrInventory, eData, plr.toolgun);
            
            PlayerNetworkManager plrNetwork = player.GetComponent<PlayerNetworkManager>();
            leave.onClick.AddListener(() => plrNetwork.Kick());

            crosshair.Init(movement, plrInventory, plrEntity);
        }
    }
}
