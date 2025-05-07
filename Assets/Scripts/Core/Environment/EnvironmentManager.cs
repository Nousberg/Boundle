using Assets.Scripts.Network;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Core.Environment
{
    public class EnvironmentManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image darkTint;
        [SerializeField] private AudioSource birdsSound;
        [SerializeField] private AudioSource cricketsSound;
        [SerializeField] private AnimationCurve birdsCurve;
        [SerializeField] private AnimationCurve cricketsCurve;
        [SerializeField] private Gradient ambientColor;
        [SerializeField] private Gradient skyColor;
        [SerializeField] private Gradient tintColor;
        [SerializeField] private Gradient directionalColor;
        [SerializeField] private Gradient fogColor;

        [field: Header("Properties")]
        [field: SerializeField, Range(0f, 23.59f)] public float DayTime { get; private set; }
        [SerializeField, Min(0f)] private float timeModifier;
        [SerializeField, Min(0f)] private float skySpeed;
        [SerializeField] private float fogDensity;
        [SerializeField] bool stopTime;
        [SerializeField] private float environmentNetworkUpdateDelay;

        private List<FogData> fogs = new List<FogData>();

        public void Start() => StartCoroutine(EnvironmentNetworkUpdater());

        private void Update()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (!stopTime)
                {
                    DayTime = DayTime + (Time.deltaTime / 240f) * 24f * timeModifier;
                    DayTime %= 24f;
                }

                SetAmbient(DayTime);
            }
            else
            {
                DayTime = Convert.ToSingle(PhotonNetwork.CurrentRoom.CustomProperties[Connector.ROOM_HASHTABLE_TIME_KEY]);
                SetAmbient(DayTime);
            }
        }
        private IEnumerator EnvironmentNetworkUpdater()
        {
            while (true)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable()
                    {
                        { Connector.ROOM_HASHTABLE_TIME_KEY, DayTime }
                    };
                
                    PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
                }
                yield return new WaitForSeconds(environmentNetworkUpdateDelay);
            }
        }
        private void SetAmbient(float _dayTime)
        {
            float time = _dayTime / 24f;
            float fogIntensity = fogDensity;
            Color targetFogColor = fogColor.Evaluate(time);

            foreach (var fog in fogs)
            {
                float fogWeight = fog.weight / fogs.Sum(n => n.weight);

                targetFogColor = Color.Lerp(targetFogColor, fog.color, fogWeight);
                fogIntensity = Mathf.Lerp(fogIntensity, fog.intensity, fogWeight);
            }

            RenderSettings.ambientLight = ambientColor.Evaluate(time);
            RenderSettings.fogColor = targetFogColor;
            RenderSettings.fogDensity = fogIntensity;
            RenderSettings.sun.transform.localRotation = Quaternion.Euler(new Vector3((time * 360f) - 90f, 0f, 0));
            RenderSettings.sun.color = directionalColor.Evaluate(time);

            if (!stopTime)
                RenderSettings.skybox.SetFloat("_Rotation", RenderSettings.skybox.GetFloat("_Rotation") + Time.deltaTime * timeModifier * skySpeed);

            RenderSettings.skybox.SetColor("_Tint", skyColor.Evaluate(time));

            darkTint.color = tintColor.Evaluate(time);

            cricketsSound.volume = cricketsCurve.Evaluate(time);
            birdsSound.volume = birdsCurve.Evaluate(time);
        }

        public void BlendFog(FogData fog) => fogs.Add(fog);
        public void RemoveFog(FogData fog) => fogs.Remove(fog);
        public bool ContainsFog(FogData fog) => fogs.Contains(fog);

        public class FogData
        {
            public float weight;
            public float intensity;
            public Color color;

            public FogData(float weight, Color color, float intensity)
            {
                this.weight = weight;
                this.color = color;
                this.intensity = intensity;
            }
        }
    }
}
