using System;
using System.Threading;
using VContainer.Unity;
using UniRx;
using Cysharp.Threading.Tasks;


namespace Assets._MyGame.Script.InGame.HandWrite
{
    /// <summary>
    /// 手書き領域Presenter
    /// </summary>
    public class HandWriteAreaPresenter : IInitializable, IDisposable
    {
        private HandWriteAreaView _handWriteAreaView;
        private ReadyUsecase _readyUsecase;
        private InGameUsecase _inGameUsecase;
        private HandWriter _handWriter;

        private readonly CompositeDisposable _disposable = new();

        public HandWriteAreaPresenter(
            HandWriteAreaView handWriteAreaView,
            ReadyUsecase readyUsecase,
            InGameUsecase inGameUsecase,
            HandWriter handWriter
            )
        {
            _handWriteAreaView = handWriteAreaView;
            _readyUsecase = readyUsecase;
            _inGameUsecase = inGameUsecase;
            _handWriter = handWriter;
        }
        public void Initialize()
        {

            _readyUsecase.OnComplete.Subscribe(_ => _handWriteAreaView.HidePrepareMessage())
                .AddTo(_disposable);

            _inGameUsecase.OnStart.Subscribe(_ => _handWriteAreaView.HideMessageArea())
                .AddTo(_disposable);

            _handWriteAreaView.DeleteButton.OnClickAsObservable()
                .Subscribe(_ => _handWriter.ClearBuffer())
                .AddTo(_disposable);

        }
               

        public async UniTask PlayAnimationWhenStart(CancellationToken cancellation)
        {
            await _handWriteAreaView.SetAnimationOnStart(cancellation);
        }


        public void Dispose() => _disposable.Dispose();
    }
}