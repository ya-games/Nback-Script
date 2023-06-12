using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx.Triggers;

namespace Assets._MyGame.Script.Title
{
    /// <summary>
    /// プライバシーポリシー表示用View
    /// </summary>
    public class PrivacyPolicyScrollView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Image _background; //メッセージの領域

        public IObservable<PointerEventData> OnClickAsObservable() => _background.OnPointerDownAsObservable();

        public void SetDefault()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }

    }


}