using UniRx;

namespace Assets._MyGame.Script.Audio
{
    public class AudioSettingsService
    {
        public IReadOnlyReactiveProperty<AudioVolume> BgmVolume => _bgmVolume;
        public IReadOnlyReactiveProperty<AudioVolume> SeVolume => _seVolume;

        private readonly ReactiveProperty<AudioVolume> _bgmVolume = new(new AudioVolume(0.3f));
        private readonly ReactiveProperty<AudioVolume> _seVolume = new(new AudioVolume(0.5f));

        public void SetBgmVolume(AudioVolume volume) => _bgmVolume.Value = volume;

        public void SetSeVolume(AudioVolume volume) => _seVolume.Value = volume;
    }

}