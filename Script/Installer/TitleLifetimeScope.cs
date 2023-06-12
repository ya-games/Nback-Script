using VContainer;
using VContainer.Unity;
using UnityEngine;
using Assets._MyGame.Script.GameInformation;
using Assets._MyGame.Script.Ads;
using Assets._MyGame.Script.Title;

namespace Assets._MyGame.Script.InGame.Installer
{
    /// <summary>
    /// VContainer Title用のLifetimeScope
    /// </summary>
    public class TitleLifetimeScope : LifetimeScope
    {

        [SerializeField]
        private AdsConfigSO _adsConfigSO;

        [SerializeField]
        private GameInfoSO _gameInfoSO;

        protected override void Configure(IContainerBuilder builder)
        {

            builder.RegisterInstance(_adsConfigSO);
            builder.Register<AdsManager>(Lifetime.Scoped);

            builder.RegisterInstance(_gameInfoSO);

            builder.RegisterComponentInHierarchy<PrivacyPolicyScrollView>();
            builder.RegisterComponentInHierarchy<TitleView>();
            builder.RegisterEntryPoint<TitlePresenter>();


            //builder.RegisterBuildCallback(resolver =>
            //{
            //    //resolver.Resolve<AudioPlayer>().PlayBgm(0);
            //});
        }
    }
}