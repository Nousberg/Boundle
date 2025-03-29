using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Abilities;
using Assets.Scripts.Inventory;
using Assets.Scripts.Inventory.DynamicData;
using Assets.Scripts.Inventory.Scriptables;
using Assets.Scripts.Movement;
using Assets.Scripts.Network;
using Assets.Scripts.Ui.Core;
using Assets.Scripts.Ui.Player.Scriptables;
using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Newtonsoft.Json;
using Assets.Scripts.Interactables;

namespace Assets.Scripts.Ui.Player
{
    public class GameVisualManager : MonoBehaviourPunCallbacks
    {
        [Header("References")]
        [SerializeField] private List<SpawnableData> spawnables = new List<SpawnableData>();
        [SerializeField] private List<StatusData> statuses = new List<StatusData>();
        [SerializeField] private TextMeshProUGUI masterVolText;
        [SerializeField] private TextMeshProUGUI sfxVolText;
        [SerializeField] private TextMeshProUGUI environmentVolText;
        [SerializeField] private TextMeshProUGUI musicVolText;
        [SerializeField] private AudioMixer audioController;
        [SerializeField] private Slider resolutionSlider;
        [SerializeField] private Slider masterVolSlider;
        [SerializeField] private Slider environmentVolSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Button ban;
        [SerializeField] private Button kick;
        [SerializeField] private Button spectate;
        [SerializeField] private Button playerPropertiesMenu;
        [SerializeField] private Button serverPropertiesMenu;
        [SerializeField] private GameObject spawnablePrefab;
        [SerializeField] private GameObject globalChatContainer;
        [SerializeField] private GameObject chatContainer;
        [SerializeField] private GameObject spectateCameraPrefab;
        [SerializeField] private WindowController windows;
        [SerializeField] private GameObject objectMenu;
        [SerializeField] private GameObject playerElementPrefab;
        [SerializeField] private Transform objectsParent;
        [SerializeField] private Transform playersParent;
        [SerializeField] private CanvasGroup inventoryChangeAlert;
        [SerializeField] private TextMeshProUGUI quality;
        [SerializeField] private TextMeshProUGUI inventoryChangeText;
        [SerializeField] private Volume regularGraphics;
        [SerializeField] private Volume deadGraphics;
        [SerializeField] private Volume menuEffects;
        [SerializeField] private TextMeshProUGUI resolutionLabel;
        [SerializeField] private Image bloodScreen;
        [SerializeField] private GameObject chatBox;
        [SerializeField] private CanvasGroup mainUiGroup;
        [SerializeField] private CanvasGroup settingsUiGroup;
        [SerializeField] private Image ammoBackground;
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private GameObject ammoContainer;
        [SerializeField] private Transform itemIconsParent;

        [Header("Blood Screen Properties")]
        [SerializeField] private AnimationCurve pulsationCurve;
        [Range(0f, 1f)][SerializeField] private float maxPulsationRate;
        [Range(0f, 1f)][SerializeField] private float onDamageOvershoot;
        [Range(0f, 1f)][SerializeField] private float minWeight;
        [SerializeField] private float weightSpeed;
        [SerializeField] private float mainUiSpeed;
        [SerializeField] private float maxPulsationAmplitude;
        [SerializeField] private float pulsationRate;
        [SerializeField] private float pulsationAmplitude;
        [SerializeField] private float transperencySpeed;

        [Header("Crosshair Properties")]
        [Range(0, 1f)][SerializeField] private float minCrosshairTransperency;
        [Range(0, 1f)][SerializeField] private float healthMinCrosshairSpacing;
        [Range(0, 1f)][SerializeField] private float minCrosshairCellSize;
        [Range(0, 1f)][SerializeField] private float minCrosshairSpacing;
        [Range(0, 1f)][SerializeField] private float healthMinCrosshairCellSize;
        [Range(0, 1f)][SerializeField] private float defaultCorsshairTransperency;
        [SerializeField] private float crosshairLerpSpeed;
        [SerializeField] private float velocityMultiplier;

        [Header("Chat Box Properties")]
        [SerializeField] private KeyCode chatOpenBind;

        [Header("Settings Menu Animation")]
        [SerializeField] private Ease openEase;
        [SerializeField] private Ease closeEase;
        [Min(0f)][SerializeField] private float closeDuration;
        [Min(0f)][SerializeField] private float openDuration;
        [Range(0f, 1f)][SerializeField] private float closedStateScaleOffset;
        [SerializeField] private Vector2 closedStatePositionOffset;

        [Header("Inventory Properties")]
        [SerializeField] private Ease appearEase;
        [SerializeField] private float itemAppearAnimDuration;
        [Range(0f, 1f)][SerializeField] private float selectedItemOpacity;

        private RectTransform settingsRect => settingsUiGroup.GetComponent<RectTransform>();
        private InputMachine inputMachine => GetComponent<InputMachine>();

        private PlayerMovementLogic playerMovement;
        private InventoryDataController inventory;
        private EntityNetworkData networkData;
        private ToolgunDataController toolgun;
        private GameObject plrCam;
        private FlyAbility fly;
        private Entity player;

        private List<GameObject> icons = new List<GameObject>();
        private List<GameObject> spawnableInst = new List<GameObject>();
        private List<GameObject> players = new List<GameObject>();

        private int spectateTarget;
        private Interactable currentInteractable;
        private SettingsPreset settingsPreset;
        private Cinemachine.CinemachineFreeLook currentSpecCam;
        private int currentCrosshair;
        private Sequence settingsAnim;
        private Sequence itemAnim;
        private bool canUpdateBloodscreen = true;
        private float defaultMainUiAlpha;
        private float defaultSettingsUiAplha;
        private float defaultItemAlertOpacity;
        private Vector2 defaultCrosshairSpacing;
        private Vector2 defaultCrosshairCellSize;
        private bool settingsEnabled;
        private bool objectMenuEnabled;
        private bool isChatOpen;
        private bool spectator;
        private DepthOfField menuBlur;

        private string settingsSavePath => Path.Combine(Application.persistentDataPath, "settings.json");

        public void Init(GameObject plrCam, Entity player, FlyAbility fly, PlayerMovementLogic playerMovement, InventoryDataController inventory, EntityNetworkData networkData, ToolgunDataController toolgun)
        {
            menuBlur = menuEffects.profile.components.Find(n => n.GetType() == typeof(DepthOfField)) as DepthOfField;

            this.plrCam = plrCam;
            this.toolgun = toolgun;
            this.fly = fly;
            this.networkData = networkData;
            this.inventory = inventory;
            this.player = player;
            this.playerMovement = playerMovement;

            defaultItemAlertOpacity = inventoryChangeAlert.alpha;

            defaultMainUiAlpha = mainUiGroup.alpha;
            defaultSettingsUiAplha = settingsUiGroup.alpha;

            player.OnDamageTaken += (dmg) =>
            {
                StopCoroutine(nameof(OvershootBloodscreen));
                StartCoroutine(OvershootBloodscreen(dmg));
            };

            player.OnDeath += () =>
            {
                ToggleChat(false);
                ToggleSettings(false);
                StartCoroutine(DeadProfile());
            };

            networkData.OnRightsEdit += PropertiesUsability;

            inventory.OnItemSwitched += UpdateInventory;
            inventory.OnItemAdded += (name) => ShowInventoryUpdate("+ " + name);
            inventory.OnItemRemoved += (name) => ShowInventoryUpdate("- " + name);

            ToggleCursor(false);
            UpdateInventory();
            PropertiesUsability();
            StartCoroutine(RefreshPlayerList());

            foreach (var obj in spawnables)
            {
                GameObject inst = Instantiate(spawnablePrefab, objectsParent);

                inst.name = obj.name;

                SpawnableElement el = inst.GetComponent<SpawnableElement>();
                el.Name.text = obj.name;
                el.Icon.sprite = obj.Icon;

                spawnableInst.Add(inst);

                inst.GetComponent<Button>().onClick.AddListener(() => { toolgun.selectedObjectId = obj.Id; } );
            }

            if (File.Exists(settingsSavePath))
                try
                {
                    settingsPreset = JsonUtility.FromJson<SettingsPreset>(File.ReadAllText(settingsSavePath));
                }
                catch
                {
                    settingsPreset = new SettingsPreset();
                }
            else
                settingsPreset = new SettingsPreset();

            musicVolText.text = "Music: " + (int)settingsPreset.musicVol;
            sfxVolText.text = "Sfx: " + (int)settingsPreset.sfxVol;
            masterVolText.text = "Master: " + (int)settingsPreset.masterVol;
            environmentVolText.text = "Environment: " + (int)settingsPreset.environmentVol;
            quality.text = "Quality: " + settingsPreset.quality;
            resolutionLabel.text = "Resolution: " + settingsPreset.resolution;

            musicSlider.value = settingsPreset.musicVol;
            masterVolSlider.value = settingsPreset.masterVol;
            environmentVolSlider.value = settingsPreset.masterVol;
            sfxSlider.value = settingsPreset.sfxVol;
            resolutionSlider.value = settingsPreset.resolution;
            masterVolSlider.value = settingsPreset.masterVol;
            musicSlider.value = settingsPreset.musicVol;
            sfxSlider.value = settingsPreset.sfxVol;

            if (!PhotonNetwork.OfflineMode)
                serverPropertiesMenu.onClick.AddListener(() => windows.Open(19, 99));

            ApplySettingsPreset();
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) => StartCoroutine(RefreshPlayerList());
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            StartCoroutine(RefreshPlayerList());

            if (otherPlayer.ActorNumber == spectateTarget)
                StopSpectating();
        }
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(Connector.ROOM_BANLIST_KEY))
            {
                List<string> banned = JsonConvert.DeserializeObject<List<string>>(propertiesThatChanged[Connector.ROOM_BANLIST_KEY].ToString());

                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (banned.Contains(player.CustomProperties[Connector.PLAYER_ID_KEY].ToString()))
                    {
                        foreach (PhotonView view in FindObjectsOfType<PhotonView>())
                        {
                            if (view.Owner.ActorNumber == player.ActorNumber)
                                view.RPC(nameof(PlayerNetworkManager.Kick), player, "banned");
                        }
                    }
                }
            }
        }

        private IEnumerator RefreshPlayerList()
        {
            yield return new WaitForSeconds(2.5f);

            foreach (var plr in players)
                Destroy(plr);

            players.Clear();

            foreach (var plr in PhotonNetwork.PlayerList)
            {
                GameObject plrInfo = Instantiate(playerElementPrefab, playersParent);
                players.Add(plrInfo);

                Button plrButton = plrInfo.GetComponent<Button>();

                plrButton.onClick.RemoveAllListeners();

                if (plr.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber && (networkData.EntityRights >= EntityNetworkData.Rights.Absolute || PhotonNetwork.IsMasterClient))
                {
                    plrButton.onClick.AddListener(() =>
                    {
                        windows.Open(6, 99);

                        ban.onClick.RemoveAllListeners();
                        kick.onClick.RemoveAllListeners();
                        spectate.onClick.RemoveAllListeners();

                        ban.onClick.AddListener(() =>
                        {
                            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(Connector.ROOM_BANLIST_KEY, out object banListObj))
                            {
                                List<string> banned = JsonConvert.DeserializeObject<List<string>>(banListObj.ToString());
                                banned.Add(plr.CustomProperties[Connector.PLAYER_ID_KEY].ToString());


                                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
                                {
                                    { Connector.ROOM_BANLIST_KEY, JsonConvert.SerializeObject(banned) }
                                };
                                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                            }
                        });

                        kick.onClick.AddListener(() =>
                        {
                            foreach (PhotonView view in FindObjectsOfType<PhotonView>())
                            {
                                if (view.Owner.ActorNumber == plr.ActorNumber)
                                    view.RPC(nameof(PlayerNetworkManager.Kick), plr, "not specified");
                            }
                        });

                        spectate.onClick.AddListener(() =>
                        {
                            foreach (var roomPlayer in FindObjectsOfType<PlayerNetworkManager>())
                            {
                                if (roomPlayer.view.Owner.ActorNumber == plr.ActorNumber)
                                {
                                    if (currentSpecCam == null)
                                    {
                                        GameObject specCamInstance = Instantiate(spectateCameraPrefab);
                                        Cinemachine.CinemachineFreeLook specCam = specCamInstance.GetComponent<Cinemachine.CinemachineFreeLook>();
                                        specCam.Follow = roomPlayer.transform;
                                        specCam.LookAt = roomPlayer.transform;

                                        currentSpecCam = specCam;
                                    }
                                    else
                                    {
                                        currentSpecCam.gameObject.SetActive(true);
                                        currentSpecCam.Follow = roomPlayer.transform;
                                        currentSpecCam.LookAt = roomPlayer.transform;
                                    }

                                    spectator = true;
                                    plrCam.SetActive(false);
                                    spectateTarget = plr.ActorNumber;
                                }
                            }
                        });

                        ban.interactable = !plr.IsMasterClient;
                        kick.interactable = !plr.IsMasterClient;
                    });
                }
                else
                    plrButton.onClick.AddListener(() => windows.Close(6));

                PlayerElement plrData = plrInfo.GetComponent<PlayerElement>();
                plrData.Name.text = plr.NickName;

                if (plr.IsMasterClient)
                {
                    plrData.Status.sprite = statuses.Find(n => n.Identifier == 0).Icon;
                    plrData.Status.enabled = true;
                    continue;
                }

                if (plr.CustomProperties.TryGetValue(Connector.PLAYER_STATUS_KEY, out object statusObj) && statusObj is int statusId)
                {
                    StatusData status = statuses.Find(n => n.Identifier == statusId);
                    if (status != null)
                    {
                        plrData.Status.sprite = status.Icon;
                        plrData.Status.enabled = true;
                    }
                }
            }
        }
        private IEnumerator DeadProfile()
        {
            while (player.Health == 0f)
            {
                yield return null;
                deadGraphics.weight = Mathf.Lerp(deadGraphics.weight, 1f, weightSpeed * Time.deltaTime);
                mainUiGroup.alpha = Mathf.Lerp(mainUiGroup.alpha, 0f, mainUiSpeed * Time.deltaTime);
            }

            while (deadGraphics.weight > minWeight)
            {
                yield return null;
                deadGraphics.weight = Mathf.Lerp(deadGraphics.weight, 0f, weightSpeed * Time.deltaTime);
                mainUiGroup.alpha = Mathf.Lerp(mainUiGroup.alpha, 1f, mainUiSpeed * Time.deltaTime);
            }

            deadGraphics.weight = 0f;
            mainUiGroup.alpha = 1f;
        }
        private void PropertiesUsability()
        {
            bool canUseProperties = networkData.EntityRights >= EntityNetworkData.Rights.Moderator || PhotonNetwork.IsMasterClient;

            playerPropertiesMenu.interactable = canUseProperties;
            serverPropertiesMenu.interactable = canUseProperties && !PhotonNetwork.OfflineMode;

            if (!canUseProperties)
            {
                windows.Close(3);
                windows.Close(6);
                windows.Close(19);
            }

            fly.ToggleUsability(canUseProperties);
        }
        private void ShowInventoryUpdate(string itemName)
        {
            if (itemAnim != null)
                itemAnim.Kill();

            inventoryChangeAlert.gameObject.SetActive(true);

            inventoryChangeAlert.alpha = defaultItemAlertOpacity;
            inventoryChangeText.text = itemName;

            itemAnim = DOTween.Sequence();

            itemAnim
                .AppendInterval(itemAppearAnimDuration)
                .Append(
                inventoryChangeAlert
                .DOFade(0f, itemAppearAnimDuration / 3f));

            itemAnim
                .OnComplete(() => inventoryChangeAlert.gameObject.SetActive(false))
                .SetEase(appearEase)
                .SetUpdate(UpdateType.Normal, true);
        }
        private void UpdateInventory()
        {
            foreach (var icon in icons)
                Destroy(icon);

            icons.Clear();

            foreach (var item in inventory.GetItems)
            {
                GameObject icon = new GameObject(item.data.name);
                Image img = icon.AddComponent<Image>();
                img.sprite = item.data.Icon;
                img.preserveAspect = true;

                if (ReferenceEquals(inventory.GetItems[inventory.CurrentItemIndex], item))
                    img.color = new Color(img.color.r, img.color.g, img.color.b, selectedItemOpacity);

                icon.AddComponent<Button>().onClick.AddListener(() => inventory.SwitchItem(inventory.GetItems.IndexOf(item)));
                icon.transform.SetParent(itemIconsParent, false);

                icons.Add(icon);

                ItemDataController itemController = inventory.AllInGameItems.Find(n => n.BaseData.Id == item.data.Id);

                if (itemController != null)
                {
                    if (itemController is WeaponDataController weapon)
                    {
                        weapon.OnAmmoChanged -= UpdateItemInfo;
                        weapon.OnAmmoChanged += UpdateItemInfo;
                    }
                }
            }

            UpdateItemInfo();
        }
        private void UpdateItemInfo()
        {
            if (inventory.GetItems[inventory.CurrentItemIndex] is DynamicWeaponData weapon && weapon.data is BaseWeaponData baseWeapon && baseWeapon.MyType != BaseWeaponData.WeaponType.Melee)
            {
                ammoText.text = weapon.currentAmmo.ToString() + " / " + weapon.overallAmmo.ToString();
                ammoBackground.fillAmount = (float)weapon.currentAmmo / weapon.overallAmmo;

                ammoContainer.SetActive(true);
            }
            else
                ammoContainer.SetActive(false);
        }
        private void ToggleCursor(bool state)
        {
            Cursor.visible = state;
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        }
        private void ToggleChat(bool value)
        {
            if (value)
                inputMachine.BlockInput(true);

            isChatOpen = value;
            chatBox.SetActive(value);
            mainUiGroup.gameObject.SetActive(!value);
            chatContainer.SetActive(!value);
            globalChatContainer.SetActive(value);
        }
        private void ToggleSettings(bool value)
        {
            menuBlur.active = value;

            if (value)
            {
                inputMachine.BlockInput(true);
                ToggleChat(false);
            }

            settingsEnabled = value;

            ToggleCursor(value);

            StartSettingsAnimation();
        }
        private void StartSettingsAnimation()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

            settingsAnim.Kill();

            if (!settingsEnabled)
            {
                mainUiGroup.gameObject.SetActive(true);

                settingsAnim = DOTween.Sequence();

                settingsAnim.Append(
                    mainUiGroup.DOFade(defaultMainUiAlpha, closeDuration));

                settingsAnim.Join(
                    settingsUiGroup.DOFade(0f, closeDuration));

                settingsAnim.Join(
                    settingsRect.DOScale(Vector2.one * closedStateScaleOffset, closeDuration));

                settingsAnim.Join(
                    settingsRect.DOMove(screenCenter * closedStatePositionOffset, closeDuration));

                settingsAnim.OnComplete(() => { 
                    settingsUiGroup.gameObject.SetActive(false);
                } )
                        .SetUpdate(UpdateType.Normal, true)
                        .SetEase(closeEase);
            }
            else
            {
                settingsUiGroup.gameObject.SetActive(true);

                settingsAnim = DOTween.Sequence();

                settingsAnim.Append(
                    settingsRect.DOScale(Vector2.one, openDuration));

                settingsAnim.Join(
                    mainUiGroup.DOFade(0f, openDuration));

                settingsAnim.Join(
                    settingsUiGroup.DOFade(defaultSettingsUiAplha, openDuration));

                settingsAnim.Join(
                    settingsRect.DOMove(screenCenter, openDuration));

                settingsAnim.OnComplete(() => {
                    mainUiGroup.gameObject.SetActive(false);
                })
                        .SetEase(openEase)
                        .SetUpdate(UpdateType.Normal, true);
            }
        }

        public void SwitchAllowDamage(bool state)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { Connector.ROOM_HASHTABLE_ALLOW_DAMAGE_KEY, state }
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        private IEnumerator OvershootBloodscreen(float damage)
        {
            canUpdateBloodscreen = false;

            float targetTransperency = Mathf.Min(bloodScreen.color.a + (damage * onDamageOvershoot) / player.BaseHealth, 1f);

            while (targetTransperency - bloodScreen.color.a > 0.1f)
            {
                Color targetBloodscreenColor = bloodScreen.color;
                targetBloodscreenColor.a = targetTransperency;

                bloodScreen.color = Color.Lerp(bloodScreen.color, targetBloodscreenColor, transperencySpeed * Time.deltaTime);

                yield return null;
            }

            canUpdateBloodscreen = true;
        }
        private void Update()
        {
            if (player == null || player.Health == 0f)
                return;

            if (canUpdateBloodscreen)
            {
                float healthAspect = player.Health / player.BaseHealth;
                float baseAlpha = 1f - healthAspect;
                float pulseTime = Time.time * pulsationRate;
                float healthPulseRate = Mathf.Lerp(maxPulsationRate, 0f, healthAspect);

                float pulsation = pulsationCurve.Evaluate(Mathf.Sin(pulseTime * healthPulseRate)) * pulsationAmplitude * Mathf.Clamp(baseAlpha, 0f, maxPulsationAmplitude);

                Color targetBloodScreenColor = bloodScreen.color;
                targetBloodScreenColor.a = Mathf.Clamp01(baseAlpha - pulsation);

                bloodScreen.color = Color.Lerp(bloodScreen.color, targetBloodScreenColor,
                    transperencySpeed * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                currentInteractable?.DisableInteract();

                inputMachine.BlockInput(false);
                ToggleCursor(false);

                if (!isChatOpen && !objectMenuEnabled)
                    ToggleSettings(!settingsEnabled);

                if (!settingsEnabled)
                    ToggleObjectMenu(false);

                ToggleChat(false);
            }
            else if (Input.GetKeyDown(KeyCode.Space) && spectator && !isChatOpen && !objectMenuEnabled && !settingsEnabled)
                StopSpectating();
            else if (Input.GetKeyDown(chatOpenBind))
            {
                if (!settingsEnabled && !objectMenuEnabled)
                {
                    ToggleChat(true);
                    ToggleCursor(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!settingsEnabled && !isChatOpen)
                    ToggleObjectMenu(!objectMenuEnabled);
            }
        }
        private void StopSpectating()
        {
            plrCam.SetActive(true);
            currentSpecCam.gameObject.SetActive(false);

            spectator = false;
            spectateTarget = -1;
        }
        private void ToggleObjectMenu(bool value)
        {
            ToggleCursor(value);

            menuBlur.active = value;
            inputMachine.BlockInput(value);
            mainUiGroup.gameObject.SetActive(!value);
            objectMenu.SetActive(value);
            objectMenuEnabled = value;
        }
        public void ApplySettingsPreset()
        {
            File.WriteAllText(settingsSavePath, JsonUtility.ToJson(settingsPreset));

            audioController.SetFloat("sfx", Mathf.Lerp(-80f, 0f, settingsPreset.sfxVol / 100f));
            audioController.SetFloat("music", Mathf.Lerp(-80f, 0f, settingsPreset.musicVol / 100f));
            audioController.SetFloat("master", Mathf.Lerp(-80f, 0f, settingsPreset.masterVol / 100f));
            audioController.SetFloat("environment", Mathf.Lerp(-80f, 0f, settingsPreset.environmentVol / 100f));

            QualitySettings.SetQualityLevel(settingsPreset.quality);

            switch (settingsPreset.quality)
            {
                case 2:
                    regularGraphics.enabled = true;
                    break;
                default:
                    regularGraphics.enabled = false;
                    break;
            }

            UniversalRenderPipeline.asset.renderScale = settingsPreset.resolution / 100f;
        }
        public void SearchObject(string objName)
        {
            foreach (var obj in spawnableInst)
            {
                if (!obj.name.Contains(objName, StringComparison.OrdinalIgnoreCase))
                    obj.SetActive(false);
                else
                    obj.SetActive(true);
            }
        }
        public void SetGraphicsLevel(int index)
        {
            if (settingsPreset == null)
                settingsPreset = new SettingsPreset(80, index);
            else
                settingsPreset.quality = index;

            quality.text = "Quality: " + index;
        }
        public void SetMainVol(float value)
        {
            settingsPreset.masterVol = value;
            masterVolText.text = "Master: " + (int)value;
        }
        public void SetMusicVol(float value)
        {
            settingsPreset.musicVol = value;
            musicVolText.text = "Music: " + (int)value;
        }
        public void SetSfxVol(float value)
        {
            settingsPreset.sfxVol = value;
            sfxVolText.text = "Sfx: " + (int)value;
        }
        public void SetEnvironmentVol(float value)
        {
            settingsPreset.environmentVol = value;
            environmentVolText.text = "Environment: " + (int)value;
        }
        public void SetRenderScale(float value)
        {
            settingsPreset.resolution = (int)value;
            resolutionLabel.text = $"Resolution: {value}";
        }
    }
}
