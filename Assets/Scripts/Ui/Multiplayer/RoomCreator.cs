using Assets.Scripts.Network;
using Assets.Scripts.Ui.Multiplayer.Scriptables;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Multiplayer
{
    [RequireComponent(typeof(SceneInfo))]
    public class RoomCreator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image selectedSceneBanner;
        [SerializeField] private TextMeshProUGUI selectedSceneDescription;
        [SerializeField] private TextMeshProUGUI selectedScenePlayers;
        [SerializeField] private GameObject selectedSceneProperties;
        [SerializeField] private GameObject scenePanelPrefab;
        [SerializeField] private Slider playersSlider;
        [SerializeField] private Transform sceneList;
        [SerializeField] private Connector connector;

        private SceneInfo scenes => GetComponent<SceneInfo>();

        private List<GameObject> scenePanels = new List<GameObject>();
        public string buildedRoomBannerLink;
        public string buildedRoomDesc;
        public string buildedRoomName;
        public int buildedRoomPlayers;
        public int buildedRoomScene;
        public bool buildedRoomPrivate;
        public string buildedRoomPassword = string.Empty;

        private void Start()
        {
            playersSlider.maxValue = Connector.ROOM_MAX_PLAYERS;
            ShowMaxPlayers((int)playersSlider.value);

            playersSlider.onValueChanged.AddListener(value => SetBuildedRoomMaxPlayers((int)value));
            ShowAviableScenes();
        }
        private void ShowAviableScenes()
        {
            scenePanels.ForEach(n => Destroy(n));
            scenePanels.Clear();

            foreach (var scene in scenes.Scenes)
            {
                GameObject sPrefab = Instantiate(scenePanelPrefab, sceneList);
                SceneElement e = sPrefab.GetComponent<SceneElement>();
                e.SButton.onClick.AddListener(() => { 
                    SetBuildedRoomScene(scene.Index);
                    selectedSceneProperties.SetActive(true);
                });
                e.Icon.sprite = scene.Icon;
                e.Name.text = scene.name;

                scenePanels.Add(sPrefab);
            }
        }
        private void ShowMaxPlayers(int maxPlayers) => selectedScenePlayers.text = maxPlayers + " / " + Connector.ROOM_MAX_PLAYERS;

        public void CreateBuildedRoom()
        {
            if (buildedRoomPlayers > 1 && !string.IsNullOrEmpty(buildedRoomName))
                connector.CreateRoom(buildedRoomName, buildedRoomPlayers, buildedRoomScene, buildedRoomPrivate, buildedRoomPassword, buildedRoomDesc, buildedRoomBannerLink);
            else
                connector.CreateOfflineRoom(buildedRoomScene);
        }
        public void SetBuildedRoomMaxPlayers(int maxPlayers)
        {
            buildedRoomPlayers = maxPlayers;
            ShowMaxPlayers(maxPlayers);
        }
        public void SetBuildedRoomName(string name)
        {
            buildedRoomName = name;
        }
        public void SetBuildedRoomPassword(string password)
        {
            buildedRoomPassword = password;
        }
        public void SetBuildedRoomDescription(string text)
        {
            buildedRoomDesc = text;
        }
        public void SetBuildedRoomBanner(string link)
        {
            buildedRoomBannerLink = link;
        }
        public void SetBuildedRoomPrivate(bool state)
        {
            buildedRoomPrivate = state;
        }

        private void SetBuildedRoomScene(int scene)
        {
            buildedRoomScene = scene;
            UiSceneData sceneData = scenes.Scenes.Find(n => n.Index == scene);

            selectedSceneDescription.text = sceneData.Description;
            selectedSceneBanner.sprite = sceneData.Banner;
        }
    }
}