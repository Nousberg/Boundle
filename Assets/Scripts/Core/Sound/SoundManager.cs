using UnityEngine;
using Photon.Pun;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Sound
{
    [RequireComponent(typeof(PhotonView))]
    public class SoundManager : MonoBehaviour
    {
        [field: SerializeField] public List<SoundSource> soundSources { get; private set; } = new List<SoundSource>();
        [field: SerializeField] public List<SoundClip> soundClips { get; private set; } = new List<SoundClip>();
        [field: SerializeField] public PhotonView View { get; private set; }

        public void AddSource(SoundSource source) => soundSources.Add(source);

        [PunRPC]
        public void Play(int sourceId, int clipId, Vector3 pos, float volume, float pitch, bool loop = false, bool stopPrevious = false, bool editPos = false)
        {
            SoundSource source = soundSources.Find(n => n.id == sourceId);
            SoundClip clip = soundClips.Find(n => n.id == clipId);

            if (source == null || clip == null)
                return;

            if (stopPrevious)
                source.source.Stop();

            if (editPos)
                source.source.transform.position = pos;

            source.source.clip = clip.clip;
            source.source.volume = volume;
            source.source.pitch = pitch;
            source.source.loop = loop;

            source.source.Play();
        }
        [PunRPC]
        public void Stop(int identifier)
        {
            SoundSource sound = soundSources.Find(n => n.id == identifier);

            if (sound == null)
                return;

            sound.source.Stop();
        }

        [Serializable]
        public class SoundSource
        {
            public int id;
            public AudioSource source;

            public SoundSource(int id, AudioSource source)
            {
                this.id = id;
                this.source = source;
            }
        }
        [Serializable]
        public class SoundClip
        {
            public int id;
            public AudioClip clip;

            public SoundClip(int id, AudioClip clip)
            {
                this.id = id;
                this.clip = clip;
            }
        }
    }
}
