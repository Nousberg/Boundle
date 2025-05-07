using Assets.Scripts.Inventory;
using Photon.Pun.Demo.Cockpit.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Ui.Player
{
    public class ToolgunItemSelector : MonoBehaviour
    {
        [SerializeField] private Transform propIconsParent;
        [SerializeField] private Transform vehicleIconsParent;
        [SerializeField] private Transform serverIconsParent;
        [SerializeField] private GameObject objectInfoPrefab;

        private ToolgunDataController toolgun;
        private bool activeSearch;

        public void Init(ToolgunDataController toolgun)
        {
            this.toolgun = toolgun;
        }

        public void ShowProps()
        {
            if (!activeSearch)
            {
                vehicleIconsParent.gameObject.SetActive(false);
                serverIconsParent.gameObject.SetActive(false);
                propIconsParent.gameObject.SetActive(true);
            }
        }
    }
}
