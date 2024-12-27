using Assets.Scripts.Entities;
using Assets.Scripts.Ui.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private GameVisualManager visualManager;

        private void Awake()
        {
            visualManager.Init();
        }
    }
}