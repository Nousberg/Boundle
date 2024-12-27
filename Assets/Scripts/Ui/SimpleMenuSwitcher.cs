using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class SimpleMenuSwitcher : MonoBehaviour
    {
        [SerializeField] private List<GameObject> menus = new List<GameObject>();
        [SerializeField] private GameObject mainMenu;

        public void ToMainMenu()
        {
            foreach (var menu in menus)
                menu.SetActive(false);

            mainMenu.SetActive(true);
        }
    }
}