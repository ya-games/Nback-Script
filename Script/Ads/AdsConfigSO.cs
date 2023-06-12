using UnityEngine;

namespace Assets._MyGame.Script.Ads
{
    /// <summary>
    /// UnityAds用の設定データ
    /// </summary>

    [CreateAssetMenu(fileName = "AdsConfigSO", menuName = "Create/AdsConfigSO")]

    
    public class AdsConfigSO : ScriptableObject
    {
        [SerializeField] string _androidGameId;
        //[SerializeField] string _iOSGameId;
        [SerializeField] bool _testMode = true;
        [SerializeField] string _androidPlacementId;

        public string AndroidGameId => _androidGameId;
        //public string iOSGameId => _iOSGameId;
        public bool TestMode => _testMode;
        public string AndroidPlacementId => _androidPlacementId;

    }
}