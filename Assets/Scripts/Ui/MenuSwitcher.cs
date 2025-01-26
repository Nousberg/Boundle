using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class MenuSwitcher : MonoBehaviour
    {
        [SerializeField] private List<GameObject> menus = new List<GameObject>();
        [SerializeField] private GameObject mainMenu;

        public void DeactivateAll() => menus.ForEach(n => n.SetActive(false));
        public void ToMainMenu()
        {
            DeactivateAll();
            mainMenu.SetActive(true);
        }
    }
}