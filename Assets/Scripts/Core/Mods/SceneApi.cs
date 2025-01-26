using Assets.Scripts.Network.Chat;
using Assets.Scripts.Spawning;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core.Mods
{
    [RequireComponent(typeof(Summonables))]
    [RequireComponent(typeof(ChatInputHandler))]
    public class SceneApi : MonoBehaviour
    {
        public event Action OnUpdate;
        public event Action OnStart;

        public List<GameObject> GetObjects() => FindObjectsOfType<GameObject>().ToList();
        
        public Summonables summoner => GetComponent<Summonables>();
        public ChatInputHandler chat => GetComponent<ChatInputHandler>();

        private void Start() => OnStart?.Invoke();
        private void Update() => OnUpdate?.Invoke();
    }
}
