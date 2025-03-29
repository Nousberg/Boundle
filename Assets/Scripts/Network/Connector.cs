using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Core.Mods;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Assets.Scripts.Network
{
    [RequireComponent(typeof(ModsLoader))]
    public class Connector : MonoBehaviourPunCallbacks
    {
        public const int ROOM_MAX_PLAYERS = 20;
        public const string ROOM_HASHTABLE_BANNER_KEY = "bann";
        public const string ROOM_HASHTABLE_ALLOW_DAMAGE_KEY = "allowdamage";
        public const string ROOM_HASHTABLE_SCENE_KEY = "scene";
        public const string ROOM_HASHTABLE_DESC_KEY = "desc";
        public const string ROOM_HASHTABLE_PASS_KEY = "pass";
        public const string ROOM_HASHTABLE_PRIVATE_KEY = "private";
        public const string ROOM_BANLIST_KEY = "banned";
        public const string PLAYER_STATUS_KEY = "status";
        public const string PLAYER_ID_KEY = "id";
        public const string ROOM_HASHTABLE_MODS_KEY = "ModsData";
        public const string ADMIN_KEY = "admin";

        private const string CURRENT_ROOM_MODS_FOLDER = ModsLoader.MODS_FOLDER_PATH + "/CurrentRoom";

        [SerializeField] private ModsSynchronizer modsSyncer;

        public event Action OnRoomCreation;

        private ModsLoader mLoader => GetComponent<ModsLoader>();
        private string roomModsFolder => Path.Combine(Application.persistentDataPath, CURRENT_ROOM_MODS_FOLDER);

        private List<RoomInfo> rooms = new List<RoomInfo>();

        private void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            ConnectToMultiplayer();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList) => rooms = roomList;

        public override void OnConnectedToMaster()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode && PhotonNetwork.NetworkClientState != ClientState.JoiningLobby && !PhotonNetwork.InLobby)
                StartCoroutine(JoinLobbyWhenReady());
        }

        public override void OnJoinedRoom()
        {
            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(ROOM_HASHTABLE_MODS_KEY))
            {
                string modsJson = PhotonNetwork.CurrentRoom.CustomProperties[ROOM_HASHTABLE_MODS_KEY].ToString();
                List<string> roomHostMods = JsonConvert.DeserializeObject<List<string>>(modsJson);

                foreach (var modUrl in roomHostMods)
                    modsSyncer.Download(modUrl, roomModsFolder);
            }
        }

        public override void OnLeftRoom()
        {
            if (Directory.Exists(roomModsFolder))
                Directory.Delete(roomModsFolder, true);
        }

        private void ConnectToMultiplayer()
        {
            if (PhotonNetwork.InLobby || PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState == ExitGames.Client.Photon.PeerStateValue.Connecting)
                return;

            PhotonNetwork.ConnectUsingSettings();
        }

        public void CreateRoom(string name, int maxPlayers, int sceneId, bool privateMode, string password = "", string description = "", string bannerLink = "")
        {
            if (maxPlayers < 0 || maxPlayers > ROOM_MAX_PLAYERS)
                return;

            StartCoroutine(CreateNativeOnlineRoom(name, maxPlayers, sceneId, privateMode, password, description, bannerLink));
        }

        private IEnumerator CreateNativeOnlineRoom(string name, int maxPlayers, int sceneId, bool privateMode, string password = "", string description = "", string bannerLink = "")
        {
            yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby);

            List<string> modLinks = new List<string>();

            if (!Directory.Exists(ModsLoader.MODS_FOLDER_PATH))
                Directory.CreateDirectory(ModsLoader.MODS_FOLDER_PATH);

            foreach (var file in Directory.GetFiles(ModsLoader.MODS_FOLDER_PATH, $"*.{ModsLoader.MOD_EXTENSION}"))
            {
                if (mLoader.LoadModNative(file, true))
                {
                    //string result = modsSyncer.Upload(file);
                    //if (!string.IsNullOrEmpty(result))
                    //    modLinks.Add(result);
                }
            }

            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = maxPlayers,
                IsVisible = true,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
                {
                    { ROOM_HASHTABLE_ALLOW_DAMAGE_KEY, true },
                    { ROOM_HASHTABLE_MODS_KEY, JsonConvert.SerializeObject(modLinks) },
                    { ROOM_HASHTABLE_PRIVATE_KEY, privateMode },
                    { ROOM_HASHTABLE_PASS_KEY, password },
                    { ROOM_HASHTABLE_DESC_KEY, description },
                    { ROOM_HASHTABLE_SCENE_KEY, sceneId },
                    { ROOM_HASHTABLE_BANNER_KEY, bannerLink },
                    { ROOM_BANLIST_KEY, JsonConvert.SerializeObject(new List<string>()) }
                },
                CustomRoomPropertiesForLobby = new string[] 
                {
                    ROOM_HASHTABLE_ALLOW_DAMAGE_KEY,
                    ROOM_BANLIST_KEY,
                    ROOM_HASHTABLE_PRIVATE_KEY,
                    ROOM_HASHTABLE_MODS_KEY, 
                    ROOM_HASHTABLE_PASS_KEY, 
                    ROOM_HASHTABLE_DESC_KEY, 
                    ROOM_HASHTABLE_SCENE_KEY,
                    ROOM_HASHTABLE_BANNER_KEY
                }
            };

            PhotonNetwork.CreateRoom(name, roomOptions, TypedLobby.Default);
            PhotonNetwork.LoadLevel(sceneId);

            OnRoomCreation?.Invoke();
        }

        public void CreateOfflineRoom(int sceneIndex)
        {
            StartCoroutine(CreateNativeOfflineRoom(sceneIndex));
        }

        private IEnumerator CreateNativeOfflineRoom(int sceneIndex)
        {
            if (!PhotonNetwork.OfflineMode)
            {
                PhotonNetwork.Disconnect();
                yield return new WaitUntil(() => !PhotonNetwork.IsConnected);
                yield return new WaitForSeconds(0.5f);
            }

            PhotonNetwork.OfflineMode = true;
            yield return new WaitForSeconds(0.1f);
            PhotonNetwork.CreateRoom("dev", new RoomOptions { IsVisible = false, IsOpen = true });
            PhotonNetwork.LoadLevel(sceneIndex);

            OnRoomCreation?.Invoke();
        }
        private IEnumerator JoinLobbyWhenReady()
        {
            yield return new WaitForSeconds(0.3f);

            if (PhotonNetwork.IsConnectedAndReady)
                PhotonNetwork.JoinLobby();
        }
        public void JoinRoom(string name)
        {
            if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby)
                PhotonNetwork.JoinRoom(name);
        }
    }
}