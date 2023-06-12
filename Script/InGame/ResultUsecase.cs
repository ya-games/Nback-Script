using System;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using Assets._MyGame.Script.Exercises;
using Assets._MyGame.Script.InGame.State;
using Assets._MyGame.Script.Audio;
using Assets._MyGame.Script.GameInformation;

namespace Assets._MyGame.Script.InGame
{
    /// <summary>
    /// リザルトのUsecase
    /// </summary>
    public class ResultUsecase:IDisposable
    {
        private GameInfoSO _gameInfoSO;
        private ExerciseProvider _exerciseProvider;
        private AudioPlayer _audioPlayer;
        private InGameState _inGameState;

        private readonly Subject<(UIState, ExerciseResult)> _onStart = new();
        private readonly Subject<Unit> _onComplete = new();

        private readonly CompositeDisposable _disposable = new();

        public ResultUsecase(
            GameInfoSO gameInfo,
            AudioPlayer audioPlayer,
            ExerciseProvider exerciseProvider,
            InGameState inGameState
            )
        {
            _gameInfoSO = gameInfo;
            _audioPlayer = audioPlayer;
            _exerciseProvider = exerciseProvider;
            _inGameState = inGameState;

            //ストリームソースのDispose
            _onStart.AddTo(_disposable);
            _onComplete.AddTo(_disposable);

        }
        public IObservable<(UIState state,ExerciseResult result)> OnStart => _onStart;
        public IObservable<Unit> OnComplete => _onComplete;


        public async UniTask ShowResult(CancellationToken cancellation)
        {
            _audioPlayer.PlaySe(AudioResource.ClipName.ResultIn);

            var exerciseResult = new ExerciseResult(_exerciseProvider.ExerciseList);

            // 開始時のアニメーション処理
            var uiState = new UIState(UIStateType.PREPARATION, cancellation);
            _onStart.OnNext((uiState,exerciseResult));
            await UniTask.WaitUntil(() => uiState.StateValue == UIStateType.FINISHED, cancellationToken: cancellation);
                        
        }
        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
