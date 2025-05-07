using Assets.Scripts.Core;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class MenuSwitcher : MonoBehaviour
    {
        [SerializeField] private List<GameObject> menus = new List<GameObject>();
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject kickedWindow;
        [SerializeField] private TextMeshProUGUI kickedReason;

        public void DeactivateAll() => menus.ForEach(n => n.SetActive(false));
        public void ToMainMenu()
        {
            DeactivateAll();
            mainMenu.SetActive(true);
        }
    }
}