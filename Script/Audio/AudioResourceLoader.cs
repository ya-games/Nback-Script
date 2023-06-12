using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets._MyGame.Script.Audio
{
    public class AudioResourceLoader
    {
        private AudioResource _audioResource;

        public AudioResourceLoader(AudioResource audioResource)
        {
            _audioResource = audioResource;
        }

        public async UniTask<AudioClip> LoadAsync(AudioResource.ClipName audioName)
        {
            if (!_audioResource.Valid(audioName))
            {
                throw new OperationCanceledException();
            }

            var audioClip = _audioResource.Get(audioName);
            if (audioClip.loadState != AudioDataLoadState.Loaded)
            {
                audioClip.LoadAudioData();
            }

            await UniTask.WaitUntil(() => audioClip.loadState == AudioDataLoadState.Loaded);
            return audioClip;
        }
    }
}