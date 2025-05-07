using Assets.Scripts.Inventory;
using Assets.Scripts.Inventory.Scriptables;
using Assets.Scripts.Movement;
using UnityEngine;
using Photon.Pun;
using Assets.Scripts.Entities;

namespace Assets.Scripts.Core.Sound
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerSoundController : MonoBehaviour
    {
        [Header("Sources")]
        [SerializeField] private AudioSource wind;
        [SerializeField] private AudioSource tinnitus;
        [SerializeField] private AudioSource weaponSwitch;
        [SerializeField] private AudioSource steps;
        [SerializeField] private AudioSource landing;
        [SerializeField] private AudioSource weapon;

        [Header("References")]
        [SerializeField] private AudioReverbFilter reverbFilter;
        [SerializeField] private InventoryDataController inventory;
        [SerializeField] private PlayerMovementLogic movement;
        [SerializeField] private Entity entity;

        [Header("Properties")]
        [SerializeField] private int itemSwitchClipId;
        [SerializeField] private int itemSwitchSourceId;
        [SerializeField] private float tinnitusLerpSpeed;
        [SerializeField, Range(0f, 1f)] private float tinnitusHealthAspect;

        private PhotonView view => GetComponent<PhotonView>();
        private SoundManager soundManager;

        public void Init(SoundManager soundManager)
        {
            this.soundManager = soundManager;

            soundManager.AddSource(new SoundManager.SoundSource(901, weaponSwitch));
            soundManager.AddSource(new SoundManager.SoundSource(902, steps));
            soundManager.AddSource(new SoundManager.SoundSource(903, landing));
            soundManager.AddSource(new SoundManager.SoundSource(904, tinnitus));
            soundManager.AddSource(new SoundManager.SoundSource(905, wind));
            soundManager.AddSource(new SoundManager.SoundSource(906, weapon));

            inventory.OnItemSwitched += () => { 
                WeaponSfx(itemSwitchSourceId, itemSwitchClipId);
                UpdateReloadBase(string.Empty);
            };
            inventory.OnItemAdded += UpdateReloadBase;
            inventory.OnItemRemoved += UpdateReloadBase;

            soundManager.Play(904, 2, Vector3.zero, 1f, 1f, true);
            soundManager.Play(905, 3, Vector3.zero, 1f, 1f, true);
        }

        private void Update()
        {
            if (!view.IsMine)
                return;

            float healthAspect = entity.Health / entity.BaseHealth;

            reverbFilter.room = Mathf.Lerp(0f, -10000f, movement.isUnderwater ? 0f : healthAspect);
            reverbFilter.decayTime = Mathf.Lerp(20f, 0.1f, movement.isUnderwater ? 0f : healthAspect);

            if (healthAspect < tinnitusHealthAspect)
                tinnitus.volume = Mathf.Lerp(tinnitus.volume, 1f - healthAspect, tinnitusLerpSpeed * Time.deltaTime);
            else
                tinnitus.volume = Mathf.Lerp(tinnitus.volume, 0f, tinnitusLerpSpeed * Time.deltaTime);

            float windInfluence = movement.CurrentVelocity / 100f;

            wind.volume = windInfluence;
            wind.pitch = windInfluence;
        }
        private void UpdateReloadBase(string name)
        {
            foreach (var item in inventory.GetItems)
            {
                BaseItemData itemData = item.data;
                ItemDataController itemController = inventory.AllInGameItems.Find(n => n.BaseData.Id == itemData.Id);

                if (itemData != null && itemController != null)
                {
                    if (itemData is BaseWeaponData weaponData && itemController is WeaponDataController weaponController)
                    {
                        weaponController.OnReload -= () => WeaponSfx(weaponData.SoundSourceId, weaponData.ReloadSoundId);
                        weaponController.OnReload += () => WeaponSfx(weaponData.SoundSourceId, weaponData.ReloadSoundId);

                        weaponController.OnFire -= () => WeaponSfx(weaponData.SoundSourceId, weaponData.FireSoundId);
                        weaponController.OnFire += () => WeaponSfx(weaponData.SoundSourceId, weaponData.FireSoundId);

                        weaponController.OnOutOfAmmo -= () => WeaponSfx(weaponData.SoundSourceId, weaponData.NoAmmoSoundId);
                        weaponController.OnOutOfAmmo += () => WeaponSfx(weaponData.SoundSourceId, weaponData.NoAmmoSoundId);
                    }
                }
            }
        }
        private void WeaponSfx(int sourceId, int clipId) => soundManager.View.RPC(nameof(SoundManager.Play), RpcTarget.All, sourceId, clipId, Vector3.zero, 1f, 1f, false, false, false);
    }
}
