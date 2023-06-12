using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets._MyGame.Script.InGame.Header
{
    /// <summary>
    /// ヘッダー部View
    /// </summary>
    public class HeaderView : MonoBehaviour
    {
        [SerializeField]
        private Button _btnHome;

        [SerializeField]
        private TextMeshProUGUI _nbackLevel;

        [SerializeField]
        private TextMeshProUGUI _nbackCount;

        public Button BtnHome => _btnHome;
        public TextMeshProUGUI NbackLevel => _nbackLevel;
        public TextMeshProUGUI NbackCount => _nbackCount;


        public void SetText(int nbackLevel, int nbackCount)
        {
            _nbackLevel.text = $"{nbackLevel.ToString()}バック";
            _nbackCount.text = $"{nbackCount.ToString()}問";
        }
    }
}