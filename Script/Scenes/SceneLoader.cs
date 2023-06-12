using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Assets._MyGame.Script.Scenes
{
    /// <summary>
    /// シーンローダー
    /// </summary>
    public class SceneLoader : IDisposable
    {
        private readonly SceneTransitionView _sceneTransitionView;
        private readonly CompositeDisposable _disposable = new();

        private CancellationTokenSource cancellationTokenSource = new();

        public SceneLoader(SceneTransitionView sceneTransitionView)
        {
            _sceneTransitionView = sceneTransitionView;
        }

        public async UniTask LoadAsync(SceneDefine nextScene)
        {
            if (!Enum.IsDefined(typeof(SceneDefine), nextScene))
            {
                throw new OperationCanceledException();
            }

            await _sceneTransitionView.Show();

            var currentScene = SceneManager.GetActiveScene();

            //EventSystemやAudioListenerを遷移前にOFF
            SetActiveSomeComponent(currentScene, false);

            // 遷移先のシーンを読み込み
            await SceneManager.LoadSceneAsync((int)nextScene, LoadSceneMode.Additive)
                .ToUniTask(cancellationToken: cancellationTokenSource.Token);


            // アクティブ化
            var loadedScene = SceneManager.GetSceneByBuildIndex((int)nextScene);
            SceneManager.SetActiveScene(loadedScene);

            // 前のシーンをアンロード
            await SceneManager.UnloadSceneAsync(currentScene)
                .ToUniTask(cancellationToken: cancellationTokenSource.Token);

            await _sceneTransitionView.Hide();
        }
        private void SetActiveSomeComponent(Scene scene, bool val)
        {
            //EventSystem
            foreach (var g in scene.GetRootGameObjects())
            {
                if (g.GetComponent<EventSystem>() != null)
                {
                    g.SetActive(val);
                    break;
                }
            }

            //AudioListner
            foreach (var g in scene.GetRootGameObjects())
            {
                if (g.GetComponent<AudioListener>() != null)
                {
                    g.SetActive(val);
                    break;
                }
            }

        }

        public void Dispose()
        {
            _disposable.Dispose();
            cancellationTokenSource.Dispose();
        }
    }
}