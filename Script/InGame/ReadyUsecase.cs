using System;
using System.Linq;
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
    /// ゲーム開始時のUsecase
    /// </summary>
    public class ReadyUsecase:IDisposable
    {
        //DI
        private GameInfoSO _gameInfoSO;
        private ExerciseProvider _exerciseProvider;
        private AudioPlayer _audioPlayer;
        private InGameState _inGameState;

        //Field
        private const float _durationBase = 1.5f;
        private const float _durationDifference = 0.3f;

        private readonly Subject<UIState> _onStart = new();
        private readonly Subject<Exercise> _onSetTopExercise = new();
        private readonly Subject<Unit> _onComplete = new();
        private readonly Subject<Unit> _onHide = new();
        

        private readonly CompositeDisposable _disposable = new();


        public ReadyUsecase(
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
            _onSetTopExercise.AddTo(_disposable);
            _onComplete.AddTo(_disposable);
            _onHide.AddTo(_disposable);
        }
        public IObservable<Exercise> OnSetTopExercise => _onSetTopExercise;
        public IObservable<UIState> OnStart => _onStart;
        public IObservable<Unit> OnComplete => _onComplete;
        public IObservable<Unit> OnHide => _onHide;



        public async UniTask Ready(CancellationToken cancellation)
        {
            //次の問題を表示するまでの間隔
            float duration = _durationBase;
            if (_gameInfoSO.ExerciseSpeedLevel == GameInfoSO.Level.High) duration -= _durationDifference;
            if (_gameInfoSO.ExerciseSpeedLevel == GameInfoSO.Level.Low) duration += _durationDifference;


            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellation);

            //開始時のアニメーション処理
            var uiState = new UIState(UIStateType.PREPARATION, cancellation);
            _onStart.OnNext(uiState);

            await UniTask.WaitUntil(() => uiState.StateValue == UIStateType.FINISHED, cancellationToken: cancellation);
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellation);

            foreach (var x in Enumerable.Range(1, _gameInfoSO.NbackLevel))
            {   
                _exerciseProvider.MoveNextTopIndex();
                _onSetTopExercise.OnNext(_exerciseProvider.CurrentTopExercise);
                
                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellation);

                _onHide.OnNext(Unit.Default);

                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: cancellation);
            }

            _inGameState.SetState(InGameStateType.CAN_ANSWER); //ゲームステータス変更
            _onComplete.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
