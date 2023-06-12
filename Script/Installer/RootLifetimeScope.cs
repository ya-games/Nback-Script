using Assets._MyGame.Script.Audio;
using Assets._MyGame.Script.Scenes;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._MyGame.Script.InGame.Installer
{
    /// <summary>
    /// VContainer RootLifetimeScope
    /// </summary>
    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private AudioResource _audioResource;

        [SerializeField]
        private SceneTransitionView _transitionViewPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            AudioInstall(builder);

            //FPSを60に設定
            Application.targetFrameRate = 60;
        }

        private void AudioInstall(IContainerBuilder builder)
        {
            // Audio
            builder.Register<AudioResource>(resolver =>
            {
                var resource = Instantiate(_audioResource);
                DontDestroyOnLoad(resource.gameObject);
                return resource;
            }, Lifetime.Singleton);

            builder.Register<AudioResourceLoader>(Lifetime.Singleton);
            builder.Register<AudioPlayer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<AudioSettingsService>(Lifetime.Singleton);

            builder.RegisterComponentOnNewGameObject<BgmPlayer>(Lifetime.Singleton).DontDestroyOnLoad();
            builder.RegisterComponentOnNewGameObject<SePlayer>(Lifetime.Singleton).DontDestroyOnLoad();

            // SceneLoad
            builder.Register<SceneManagerEvents>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<SceneLoader>(Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab<SceneTransitionView>(_transitionViewPrefab, Lifetime.Singleton).DontDestroyOnLoad();
        }
    }
}
