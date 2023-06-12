using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._MyGame.Script.Audio
{
    public class AudioResource : MonoBehaviour
    {
        [SerializeField]
        private AudioClip[] _audioClips;

        public IEnumerable<AudioClip> AudioClips => _audioClips;

        public enum ClipName
        {
            Start,//開始音
            Correct,//正解音
            InCorrect, //不正解音
            ResultIn, //結果画面に入るとき
        }

        public bool Valid(ClipName name)
        {
            var id = (int)name;
            return 0 <= id && id < _audioClips.Length;
        }

        public AudioClip Get(ClipName name)
        {
            return _audioClips.ElementAt((int)name);
        }
    }
}