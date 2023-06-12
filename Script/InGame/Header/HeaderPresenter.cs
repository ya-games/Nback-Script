using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;
using Cysharp.Threading.Tasks;
using Assets._MyGame.Script.InGame;
using Assets._MyGame.Script.Exercises;
using Assets._MyGame.Script.InGame.State;
using Assets._MyGame.Script.GameInformation;
using Assets._MyGame.Script.Scenes;

namespace Assets._MyGame.Script.InGame.Header
{
    /// <summary>
    /// ヘッダー部Presenter
    /// </summary>
    public class HeaderPresenter : IInitializable, IDisposable
    {
        private GameInfoSO _gameInfoSO;
        private SceneLoader _sceneLoader;
        private HeaderView _headerView;
        private ExerciseProvider _exerciseProvider;

        private readonly CompositeDisposable _disposable = new();

        public HeaderPresenter(
            GameInfoSO gameInfoSO,
            SceneLoader sceneLoader,
            HeaderView headerView,
            ExerciseProvider exerciseProvider
            )
        {
            _gameInfoSO = gameInfoSO;
            _sceneLoader = sceneLoader;
            _headerView = headerView;
            _exerciseProvider = exerciseProvider;
        }
        public void Initialize()
        {
            _headerView.BtnHome.OnClickAsObservable().Subscribe(_ =>
            {
                _sceneLoader.LoadAsync(SceneDefine.Title).Forget();
            }).AddTo(_disposable);

            _headerView.SetText(_gameInfoSO.NbackLevel, _exerciseProvider.ExcersiseCount);

        }

        public void Dispose() => _disposable.Dispose();
    }
}