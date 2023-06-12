using System;
using VContainer.Unity;
using UniRx;
using Cysharp.Threading.Tasks;
using Assets._MyGame.Script.InGame.State;
using Assets._MyGame.Script.Exercises;
using Assets._MyGame.Script.GameInformation;
using Assets._MyGame.Script.Scenes;

namespace Assets._MyGame.Script.InGame.Result
{
    public class ResultPresenter : IInitializable, IDisposable
    {
        private GameInfoSO _gameInfoSO;
        private SceneLoader _sceneLoader;
        private ResultView _resultView;
        private ResultUsecase _resultUsecase;


        private readonly CompositeDisposable _disposable = new();

        public ResultPresenter(
            GameInfoSO gameInfoSO,
            SceneLoader sceneLoader,
            ResultView resultView,
            ResultUsecase resultUsecase
            )
        {
            _gameInfoSO = gameInfoSO;
            _sceneLoader = sceneLoader;
            _resultView = resultView;
            _resultUsecase = resultUsecase;
        }
        public void Initialize()
        {
            _resultUsecase.OnStart.Subscribe((x) =>
            {
                ShowResult(x.state, x.result).Forget();
            }).AddTo(_disposable);

            //次のレベルボタン
            _resultView.BtnNext.OnClickAsObservable().Subscribe(_ =>
            {
                _gameInfoSO.SetNextNbackLevel();
                _sceneLoader.LoadAsync(SceneDefine.InGame).Forget();
            }).AddTo(_disposable);

            //前のレベルボタン
            _resultView.BtnPrev.OnClickAsObservable().Subscribe(_ =>
            {
                _gameInfoSO.SetPrevNbackLevel();
                _sceneLoader.LoadAsync(SceneDefine.InGame).Forget();
            }).AddTo(_disposable);

            //リトライのレベルボタン
            _resultView.BtnRetry.OnClickAsObservable().Subscribe(_ =>
            {
                _sceneLoader.LoadAsync(SceneDefine.InGame).Forget();
            }).AddTo(_disposable);

            //初期値セット
            _resultView.SetDefault();
        }

        private async UniTask ShowResult(UIState state, ExerciseResult result)
        {
            _resultView.SetVisible(true);
            await _resultView.SetAnimationOnStart(state.Cancellation);
            state.StateValue = UIStateType.FINISHED;

            await _resultView.ShowAnswerResult(result, state.Cancellation);
        }

        public void Dispose() => _disposable.Dispose();
    }

}