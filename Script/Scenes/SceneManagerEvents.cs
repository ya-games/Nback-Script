using System;
using UniRx;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Assets._MyGame.Script.Scenes
{
    /// <summary>
    /// シーンのイベントをObservableとして公開するクラス
    /// </summary>
    public class SceneManagerEvents : IInitializable, IDisposable
    {
        private readonly ISubject<Scene> _onLoaded = new Subject<Scene>();
        private readonly ISubject<Scene> _onUnloaded = new Subject<Scene>();

        void IInitializable.Initialize()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnsceneUnloaded;
        }

        public IObservable<Scene> OnLoadedAsObservable() => _onLoaded;

        public IObservable<Scene> OnUnloadedAsObservable() => _onUnloaded;

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => _onLoaded.OnNext(scene);

        private void OnsceneUnloaded(Scene scene) => _onUnloaded.OnNext(scene);


        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnsceneUnloaded;
        }
    }
}