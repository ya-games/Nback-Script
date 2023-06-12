using VContainer;
using VContainer.Unity;
using UnityEngine;
using Unity.Barracuda;
using Assets._MyGame.Script.InGame;
using Assets._MyGame.Script.InGame.HandWrite;
using Assets._MyGame.Script.Exercises;
using Assets._MyGame.Script.InGame.State;
using Assets._MyGame.Script.Inference;
using Assets._MyGame.Script.GameInformation;
using Assets._MyGame.Script.InGame.Formula;
using Assets._MyGame.Script.InGame.Header;
using Assets._MyGame.Script.InGame.Result;

namespace Assets._MyGame.Script.InGame.Installer
{   
    /// <summary>
    /// VContainer InGame用のLifetimeScope
    /// </summary>
    public class InGameLifetimeScope : LifetimeScope
    {

        [SerializeField]
        private GameInfoSO _gameInfoSO;

        [SerializeField]
        private NNModel _nnModelAsset; //手書き認識用の学習済みデータ

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_nnModelAsset);
            builder.RegisterInstance(_gameInfoSO);

            builder.Register<InGameState>(Lifetime.Scoped);

            builder.RegisterComponentInHierarchy<FormulaBottomAreaView>();
            builder.RegisterEntryPoint<FormulaBottomAreaPresenter>().AsSelf();
            builder.RegisterComponentInHierarchy<FormulaTopAreaView>();
            builder.RegisterEntryPoint<FormulaTopAreaPresenter>().AsSelf();

            builder.RegisterEntryPoint<InGamePresenter>().AsSelf();

            builder.RegisterComponentInHierarchy<HandWriteAreaView>();
            builder.RegisterEntryPoint<HandWriteAreaPresenter>().AsSelf();

            builder.RegisterComponentInHierarchy<HeaderView>();
            builder.RegisterEntryPoint<HeaderPresenter>().AsSelf();

            builder.Register<ExerciseProvider>(Lifetime.Scoped);
            builder.Register<ReadyUsecase>(Lifetime.Scoped);
            builder.Register<InGameUsecase>(Lifetime.Scoped);
            builder.Register<ResultUsecase>(Lifetime.Scoped);

            builder.RegisterComponentInHierarchy<HandWriter>();
            builder.Register<NumberInference>(Lifetime.Scoped);

            builder.RegisterComponentInHierarchy<ResultView>();
            builder.RegisterEntryPoint<ResultPresenter>().AsSelf();

            builder.RegisterEntryPoint<GameManager>();


            //builder.RegisterBuildCallback(resolver =>
            //{
            //    //resolver.Resolve<AudioPlayer>().PlayBgm(0);
            //});
        }
    }
}