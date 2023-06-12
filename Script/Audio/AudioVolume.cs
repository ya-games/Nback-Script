using UnityEngine;

namespace Assets._MyGame.Script.Audio
{
    public readonly struct AudioVolume
    {
        public static AudioVolume Zero => new AudioVolume(0);

        public float Value => _value;

        private readonly float _value;

        public AudioVolume(float volume)
        {
            _value = Mathf.Clamp01(volume);
        }
    }
}
