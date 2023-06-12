using System;
using VContainer.Unity;
using UniRx;
using Cysharp.Threading.Tasks;
using Assets._MyGame.Script.Audio;
using Assets._MyGame.Script.GameInformation;
using Assets._MyGame.Script.Ads;
using Assets._MyGame.Script.Scenes;

namespace Assets._MyGame.Script.Title
{
    /// <summary>
    /// タイトル画面Presenter
    /// </summary>
    public class TitlePresenter : IInitializable, IDisposable
    {
        private readonly TitleView _titleView;
        private readonly PrivacyPolicyScrollView _privacyPolicyScrollView;
        private readonly SceneLoader _sceneLoader;
        private readonly AudioPlayer _audioPlayer;
        private readonly GameInfoSO _gameInfo;
        private readonly AdsManager _adsManager;
        private readonly CompositeDisposable _disposable = new();

        public TitlePresenter(
            TitleView titleView,
            PrivacyPolicyScrollView privacyPolicyScrollView,
            SceneLoader sceneLoader,
            AudioPlayer audioPlayer,
            AdsManager adsManager,
            GameInfoSO gameInfo
            )
        {
            _titleView = titleView;
            _privacyPolicyScrollView = privacyPolicyScrollView;
            _sceneLoader = sceneLoader;
            _audioPlayer = audioPlayer;
            _adsManager = adsManager;
            _gameInfo = gameInfo;
        }

        public void Initialize()
        {
            //初期値のセット
            _titleView.SetDefault(_gameInfo);

            //ボタンループアニメーション
            _titleView.DoScaleLoopStartButton();

            //各種ボタンのクリック時
            _titleView.OnClickStartAsObservable()
                .Subscribe(_ =>
                {
                    _gameInfo.SetNbackLevel(_titleView.NbackCount);
                    _gameInfo.ExerciseAmountLevel = _titleView.ExerciseAmountLevel;
                    _gameInfo.ExerciseSpeedLevel = _titleView.ExerciseSpeedLevel;
                    _sceneLoader.LoadAsync(SceneDefine.InGame).Forget();
                }).AddTo(_disposable);

            _titleView.OnClickNextLevelButtonAsObservable()
                .Subscribe(_ => OnClickNexeLevelButton())
                .AddTo(_disposable);

            _titleView.OnClickPrivacyPolicyAsObservable()
                .Subscribe(_ => _privacyPolicyScrollView.Show())
                .AddTo(_disposable);

            _titleView.OnClickAdsAsObservable()
                .Subscribe(_ => _adsManager.Show())
                .AddTo(_disposable);


            //プライバシーポリシー関連
            _privacyPolicyScrollView.OnClickAsObservable()
                .Subscribe(x => _privacyPolicyScrollView.Hide())
                .AddTo(_disposable);

            _privacyPolicyScrollView.SetDefault();


            //広告関連
            _adsManager.OnAdsShowComplete
                .Subscribe(x => OnAdsShowComplete())
                .AddTo(_disposable);

            _adsManager.OnAdsShowFailure
                .Subscribe(x => OnAdsShowFailure())
                .AddTo(_disposable);

            //EventSystemの重複削除
            _titleView.DestroyDupulicatedEventSystem();


        }

        private void OnClickNexeLevelButton()
        {
            //指定レベル以上の場合は広告視聴を要求
            if (_titleView.NbackCount >= GameInfoSO.RequireAdsNbackLevel)
            {
                if (_gameInfo.IsDisplayedAds)
                {
                    _titleView.LevelSelector.ForwardClick();
                }
                else
                {
                    _titleView.DoScaleAdsButton();
                }
            }
            else
            {
                _titleView.LevelSelector.ForwardClick();
            }
        }

        private void OnAdsShowComplete()
        {
            _gameInfo.IsDisplayedAds = true;
            _titleView.HideAdsErrorText();
        }
        private void OnAdsShowFailure()
        {
            _titleView.ShowAdsErrorText();
        }


        public void Dispose() => _disposable.Dispose();

    }
}