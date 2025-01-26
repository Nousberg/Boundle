using Assets.Scripts.Network;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Multiplayer
{
    [RequireComponent(typeof(SceneInfo))]
    public class RoomsVisualizer : MonoBehaviourPunCallbacks
    {
        [SerializeField] private BannerDownloader bannerInstaller;
        [SerializeField] private Transform roomsParent;
        [SerializeField] private GameObject roomInfo;
        [SerializeField] private Image roomBanner;
        [SerializeField] private TextMeshProUGUI roomDescription;
        [SerializeField] private GameObject passwordRoomPanelPrefab;
        [SerializeField] private GameObject roomPanelPrefab;
        [SerializeField] private Connector connector;

        private SceneInfo scenes => GetComponent<SceneInfo>();

        private List<GameObject> roomObjects = new List<GameObject>();
        private string selectedRoom;
        private Sprite selectedRoomBanner;

        private void Start()
        {
            bannerInstaller.OnDownloaded += (b) =>
            {
                if (selectedRoomBanner == null)
                    selectedRoomBanner = b;
            };
        }

        public override void OnRoomListUpdate(List<RoomInfo> rooms)
        {
            if (!PhotonNetwork.InLobby)
                return;

            roomObjects.ForEach(n => Destroy(n));
            roomObjects.Clear();

            foreach (RoomInfo room in rooms)
            {
                if (!room.IsVisible || room.PlayerCount < 1)
                    continue;

                string roomPass = room.CustomProperties[Connector.ROOM_HASHTABLE_PASS_KEY] == null ? string.Empty : room.CustomProperties[Connector.ROOM_HASHTABLE_PASS_KEY].ToString();

                GameObject roomElement;
                RoomElement roomData;

                if (string.IsNullOrEmpty(roomPass))
                {
                    roomElement = Instantiate(roomPanelPrefab.gameObject);

                    roomData = roomElement.GetComponent<RoomElement>();

                    roomData.ToggleButton.onClick.AddListener(() => SelectRoom(room));
                }
                else
                {
                    roomElement = Instantiate(passwordRoomPanelPrefab);

                    roomData = roomElement.GetComponent<RoomElement>();

                    roomData.Input.onEndEdit.AddListener(value => JoinRoomWithPassword(room.Name, value, roomPass));
                }

                roomObjects.Add(roomElement);
                roomElement.transform.SetParent(roomsParent, false);

                roomData.PlayersCount.text = room.PlayerCount + " / " + Connector.ROOM_MAX_PLAYERS;
                roomData.Name.text = room.Name;

                roomData.Icon.sprite = scenes.Scenes.Find(n => n.Index == (int)room.CustomProperties[Connector.ROOM_HASHTABLE_SCENE_KEY]).Icon;
            }
        }

        private void JoinRoomWithPassword(string roomName, string inputPsw, string roomPsw)
        {
            if (inputPsw == roomPsw)
                connector.JoinRoom(roomName);
        }
        private void SelectRoom(RoomInfo info)
        {
            object desc = info.CustomProperties[Connector.ROOM_HASHTABLE_DESC_KEY];
            string bannerUrl = info.CustomProperties[Connector.ROOM_HASHTABLE_BANNER_KEY].ToString();
            int sceneIndex = (int)info.CustomProperties[Connector.ROOM_HASHTABLE_SCENE_KEY];

            if (selectedRoomBanner == null)
            {
                if (!string.IsNullOrEmpty(bannerUrl))
                {
                    bannerInstaller.Download(bannerUrl);
                    roomBanner.sprite = scenes.Scenes.Find(n => n.Index == sceneIndex).Icon;
                }
                else
                {
                    roomBanner.sprite = scenes.Scenes.Find(n => n.Index == sceneIndex).Icon;
                }
            }
            else
            {
                roomBanner.sprite = info.Name == selectedRoom ? selectedRoomBanner : scenes.Scenes.Find(n => n.Index == sceneIndex).Icon;
            }

            roomDescription.text = desc != null && desc.ToString() != string.Empty
                ? desc.ToString()
                : "Default boundle server.";

            selectedRoom = info.Name;
            roomInfo.SetActive(true);
        }

        public void JoinSelectedRoom()
        {
            if (!string.IsNullOrEmpty(selectedRoom))
                connector.JoinRoom(selectedRoom);
        }
    }
}