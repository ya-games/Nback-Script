using System;
using System.Threading;
using VContainer.Unity;
using UniRx;
using Cysharp.Threading.Tasks;
using Assets._MyGame.Script.Audio;
using Assets._MyGame.Script.GameInformation;

namespace Assets._MyGame.Script.InGame
{
    /// <summary>
    /// GameManager
    /// 処理の起点となるクラス
    /// </summary>
    class GameManager :  IAsyncStartable, IDisposable
    {
        private GameInfoSO _gameInfoSO;
        private AudioPlayer _audioPlayer;                
        private ReadyUsecase _readyUsecase;
        private InGameUsecase _inGameUsecase;
        private ResultUsecase _resultUsecase;
        

        private readonly CompositeDisposable _disposable = new();
        private CancellationTokenSource _cancellationTokenSource = new();

        public GameManager(
            GameInfoSO gameInfoSO,
            AudioPlayer audioPlayer,            
            ReadyUsecase readyUsecase,
            InGameUsecase inGameUsecase,
            ResultUsecase resultUsecase
            )
        {
            _gameInfoSO = gameInfoSO;
            _readyUsecase = readyUsecase;
            _inGameUsecase = inGameUsecase;
            _resultUsecase = resultUsecase;
        }
              

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            await _readyUsecase.Ready(cancellation);

            await _inGameUsecase.StartGame(cancellation);

            await _resultUsecase.ShowResult(cancellation);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _disposable.Dispose();
        }

        
    }
}
