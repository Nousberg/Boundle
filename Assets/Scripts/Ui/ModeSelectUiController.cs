using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    public class ModeSelectUiController : MonoBehaviour
    {
        [SerializeField] private GameObject multiplayerHud;
        [SerializeField] private GameObject singleplayerHud;
        [SerializeField] private GameObject modeSelectorTab;
        [SerializeField] private GameObject serverList;
        [SerializeField] private GameObject roomInfo;
        [SerializeField] private GameObject mapInfo;
        [SerializeField] private GameObject mapSelectorSingleplayer;
        [SerializeField] private GameObject mapSelectorMultipleplayer;

        public void SelectMultiplayer()
        {
            modeSelectorTab.SetActive(false);
            multiplayerHud.SetActive(true);
        }
        public void SelectSingleplayer()
        {
            modeSelectorTab.SetActive(false);
            singleplayerHud.SetActive(true);
        }
        public void SelectRoom(int index)
        {
            roomInfo.SetActive(true);
        }
        public void SetSingleplayerMap(int index)
        {
            mapSelectorSingleplayer.SetActive(true);
        }
        public void SetMultiplayerMap(int index)
        {
            mapSelectorMultipleplayer.SetActive(true);
        }
    }
}