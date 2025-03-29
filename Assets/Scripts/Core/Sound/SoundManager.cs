using UnityEngine;

namespace Assets.Scripts.Core.Sound
{
    public static class SoundManager
    {
        public static void Play(AudioSource source, AudioClip clip, float volume, float pitch, bool loop = false, bool stopPrevious = false)
        {
            if (stopPrevious)
                source.Stop();

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = loop;

            source.Play();
        }
    }
}
