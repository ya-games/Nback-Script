using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Assets._MyGame.Script.Scenes
{
    //シーン遷移時のview
    public class SceneTransitionView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public async UniTask Show()
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            await _canvasGroup.DOFade(1f, 0.4f)
                .SetLink(gameObject)
                .Play();
        }

        public async UniTask Hide()
        {
            await _canvasGroup.DOFade(0, 0.2f)
                .SetLink(gameObject)
                .Play();

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}