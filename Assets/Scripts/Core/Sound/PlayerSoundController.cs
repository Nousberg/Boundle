using Assets.Scripts.Inventory;
using Assets.Scripts.Inventory.Scriptables;
using Assets.Scripts.Movement;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Assets.Scripts.Entities;

namespace Assets.Scripts.Core.Sound
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerSoundController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<SoundData> sfxSounds = new List<SoundData>();
        [SerializeField] private List<int> stepSounds = new List<int>();
        [SerializeField] private List<int> landSounds = new List<int>();
        [SerializeField] private AudioReverbFilter reverb;
        [SerializeField] private AudioClip tinnitusClip;
        [SerializeField] private AudioClip windClip;
        [SerializeField] private AudioSource tinnitusSource;
        [SerializeField] private AudioSource windSource;
        [SerializeField] private AudioSource stepsSource;
        [SerializeField] private Entity player;
        [SerializeField] private PlayerMovementLogic movement;
        [SerializeField] private InventoryDataController inventory;

        [Header("Properties")]
        [Range(0f, 1f)][SerializeField] private float criticalHealth;
        [SerializeField] private float minReverb;
        [SerializeField] private float maxReverb;
        [SerializeField] private float minSoundVolume;
        [SerializeField] private float tinnitusSpeed;
        [SerializeField] private float soundStopSpeed;

        private PhotonView view => GetComponent<PhotonView>();

        private float nextStep;

        public void Init()
        {
            inventory.OnItemAdded += HandleItemChange;
            inventory.OnItemRemoved += HandleItemChange;
            inventory.OnItemSwitched += () => HandleItemChange(string.Empty);

            movement.OnLanded += (speed) => { view.RPC(nameof(PlaySound), RpcTarget.All, sfxSounds.Find(n => n.id == landSounds[Random.Range(0, landSounds.Count - 1)]).id, speed / 25f, 1f, false, true); };

            SoundManager.Play(windSource, windClip, 1f, 0f, true);
            SoundManager.Play(tinnitusSource, tinnitusClip, 1f, 0f, true);
        }
        private void Update()
        {
            if (!view.IsMine)
                return;

            float healthAspect = 1f - player.Health / player.BaseHealth;
            float soundSpeed = tinnitusSpeed * Time.deltaTime;
            float healthToTinnitus = 0f;

            if (healthAspect > criticalHealth)
            {
                healthToTinnitus = Mathf.Lerp(tinnitusSource.volume, healthAspect, soundSpeed);
                reverb.room = Mathf.Lerp(reverb.room, maxReverb * healthAspect, soundSpeed);
            }
            else
            {
                healthToTinnitus = Mathf.Lerp(tinnitusSource.volume, 0f, tinnitusSpeed * Time.deltaTime);
                reverb.room = Mathf.Lerp(reverb.room, minReverb, soundSpeed);
            }

            tinnitusSource.pitch = healthToTinnitus;
            tinnitusSource.volume = healthToTinnitus;

            float velocityToWind = movement.CurrentVelocity / 100f;

            windSource.pitch = velocityToWind;
            windSource.volume = velocityToWind;

            int randomSound = stepSounds[Random.Range(0, stepSounds.Count - 1)];
            SoundData targetClip = sfxSounds.Find(n => n.id == stepSounds[Random.Range(0, stepSounds.Count - 1)]);

            if (movement.IsWaking && Time.time >= nextStep)
            {
                float runBoost = movement.IsRunning ? 1.25f : 1f;

                view.RPC(nameof(PlaySound), RpcTarget.All, targetClip.id, 1f, runBoost, false, false);

                nextStep = Time.time + runBoost;
            }
            else if (!movement.IsWaking && targetClip.source.volume > minSoundVolume)
                view.RPC(nameof(StopSound), RpcTarget.All, targetClip.id);
        }
        private void HandleItemChange(string name)
        {
            if (inventory.GetItems[inventory.CurrentItemIndex].data is BaseWeaponData baseWeapon)
            {
                WeaponDataController weapon = inventory.AllInGameItems.Find(n => n.BaseData.Id == baseWeapon.Id) as WeaponDataController;
                if (weapon != null)
                {
                    weapon.OnFire -= () => HandleFire(baseWeapon);
                    weapon.OnFire += () => HandleFire(baseWeapon);

                    weapon.OnReload -= () => HandleReload(baseWeapon);
                    weapon.OnReload += () => HandleReload(baseWeapon);

                    weapon.OnOutOfAmmo -= () => HandleNoAmmo(baseWeapon);
                    weapon.OnOutOfAmmo += () => HandleNoAmmo(baseWeapon);
                }
            }
        }
        private void HandleFire(BaseWeaponData weaponData) => view.RPC(nameof(PlaySound), RpcTarget.All, weaponData.FireSoundId, 1f, 1f, false, true);
        private void HandleReload(BaseWeaponData weaponData) => view.RPC(nameof(PlaySound), RpcTarget.All, weaponData.ReloadSoundId, 1f, 1f, false, false);
        private void HandleNoAmmo(BaseWeaponData weaponData) => view.RPC(nameof(PlaySound), RpcTarget.All, weaponData.NoAmmoSoundId, 1f, 1f, false, false);

        [PunRPC]
        private void PlaySound(int id, float volume, float pitch, bool loop, bool stopPrevious)
        {
            SoundData sound = sfxSounds.Find(n => n.id == id);
            SoundManager.Play(sound.source, sound.clip, volume, pitch, loop, stopPrevious);
        }

        [PunRPC]
        private void StopSound(int id)
        {
            SoundData sound = sfxSounds.Find(n => n.id == id);
            sound.source.volume = Mathf.Lerp(sound.source.volume, 0f, soundStopSpeed * Time.deltaTime);
            sound.source.pitch = Mathf.Lerp(sound.source.pitch, 0f, soundStopSpeed * Time.deltaTime);
        }

        [System.Serializable]
        private class SoundData
        {
            public int id;
            public AudioSource source;
            public AudioClip clip;
        }
    }
}
