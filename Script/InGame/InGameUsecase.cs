using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer.Unity;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using Assets._MyGame.Script.Exercises;
using Assets._MyGame.Script.InGame.State;
using Assets._MyGame.Script.InGame.HandWrite;
using Assets._MyGame.Script.Audio;
using Assets._MyGame.Script.GameInformation;

namespace Assets._MyGame.Script.InGame
{
    /// <summary>
    /// ゲーム中のUsecase
    /// </summary>
    public class InGameUsecase:IDisposable
    {
        //DI
        private readonly GameInfoSO _gameInfoSO;
        private readonly ExerciseProvider _exerciseProvider;
        private readonly AudioPlayer _audioPlayer;
        private readonly HandWriter _handWriter;

        //Field
        private const float _timeoutDurationBase = 2.6f;
        private const float _timeoutDurationDifference = 0.8f;
        private readonly Subject<Unit> _onStart = new();
        private readonly Subject<Exercise> _onSetTopExercise = new();
        private readonly Subject<Exercise> _onSetBottomExercise = new();        
        private readonly Subject<Unit> _onComplete = new();
        
        private readonly Subject<Exercise> _onCorrectAnswer = new();
        private readonly Subject<Exercise> _onIncorrectAnswer = new();
        private readonly Subject<Unit> _onAnswerTimeOut = new();
        private readonly Subject<Unit> _onHide = new();

        private readonly InGameState _inGameState;
        private readonly CompositeDisposable _disposable = new();

        public InGameUsecase(
            GameInfoSO gameInfo,
            AudioPlayer audioPlayer,
            ExerciseProvider exerciseProvider,
            InGameState inGameState,
            HandWriter handWriter
            )
        {
            _gameInfoSO = gameInfo;
            _audioPlayer = audioPlayer;
            _exerciseProvider = exerciseProvider;
            _inGameState = inGameState;
            _handWriter = handWriter;

            //ストリームソースのDispose
            _onStart.AddTo(_disposable);
            _onSetTopExercise.AddTo(_disposable);
            _onSetBottomExercise.AddTo(_disposable);
            _onComplete.AddTo(_disposable);
            _onCorrectAnswer.AddTo(_disposable);
            _onIncorrectAnswer.AddTo(_disposable);
            _onAnswerTimeOut.AddTo(_disposable);
            _onHide.AddTo(_disposable);

        }
        public IObservable<Exercise> OnSetTopExercise => _onSetTopExercise;
        public IObservable<Exercise> OnSetBottomExercise => _onSetBottomExercise;
        public IObservable<Unit> OnStart => _onStart;
        public IObservable<Unit> OnComplete => _onComplete;

        public IObservable<Exercise> OnCorrectAnswer => _onCorrectAnswer;
        public IObservable<Exercise> OnIncorrectAnswer => _onIncorrectAnswer;
        public IObservable<Unit> OnAnswerTimeOut => _onAnswerTimeOut;
        public IObservable<Unit> OnHide => _onHide;
                
        public async UniTask StartGame(CancellationToken cancellation)
        {
            //1問毎のタイムアウト間隔
            float timeoutDuration = _timeoutDurationBase;
            if (_gameInfoSO.ExerciseSpeedLevel == GameInfoSO.Level.High) timeoutDuration -= _timeoutDurationDifference;
            if (_gameInfoSO.ExerciseSpeedLevel == GameInfoSO.Level.Low) timeoutDuration += _timeoutDurationDifference;

            _audioPlayer.PlaySe(AudioResource.ClipName.Start);
            _onStart.OnNext(Unit.Default);
            
            while(!_exerciseProvider.Done)
            {
                //上段式を次の問題に更新
                if (_exerciseProvider.MoveNextTopIndex()) _onSetTopExercise.OnNext(_exerciseProvider.CurrentTopExercise);

                //下段式に表示する問題がなければ終了
                if (!_exerciseProvider.MoveNextBottomIndex()) break;
                
                _onSetBottomExercise.OnNext(_exerciseProvider.CurrentBottomExercise);
                _inGameState.SetState(InGameStateType.CAN_ANSWER);

                var (hasAnswerResult, answerData) = await UniTask.WhenAny(
                    _handWriter.OnCorrectAnswer.ToUniTask(useFirstValue: true, cancellationToken: cancellation),
                    UniTask.Delay(TimeSpan.FromSeconds(timeoutDuration), cancellationToken: cancellation));

                if (hasAnswerResult)
                {
                    //正解の場合(不正解の場合はHandWriterのMouseUpで制限時間まで何度でも回答可としている）
                    var exercise = _exerciseProvider.CurrentBottomExercise;
                    _onCorrectAnswer.OnNext(exercise);
                    _audioPlayer.PlaySe(AudioResource.ClipName.Correct);

                }
                else
                {
                    //タイムアウトの場合
                    _onAnswerTimeOut.OnNext(Unit.Default);
                    _audioPlayer.PlaySe(AudioResource.ClipName.InCorrect);
                }
                _inGameState.SetState(InGameStateType.ANSWERED);

                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: cancellation);
                _handWriter.ClearBuffer();//手書き領域をクリア
                await UniTask.Delay(TimeSpan.FromSeconds(0.4f), cancellationToken: cancellation);                
                _onHide.OnNext(Unit.Default);//問題切り替え時の点滅のため非表示

                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: cancellation);

            }
            _inGameState.SetState(InGameStateType.FINISHED);

            await UniTask.Delay(TimeSpan.FromSeconds(0.6f), cancellationToken: cancellation);
        }
        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
