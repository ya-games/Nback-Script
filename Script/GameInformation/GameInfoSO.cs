using System;
using UnityEngine;


namespace Assets._MyGame.Script.GameInformation
{
    [CreateAssetMenu(fileName = "GameInfoSO", menuName = "Create/GameInfoSO")]
    //ゲーム内で共有・一時保存するためのデータを管理する用のSO
    public class GameInfoSO : ScriptableObject
    {
        //[NonSerialized]
        public int NbackLevel { get; private set; } = MinNbackLevel; //NBackレベル

        [NonSerialized]
        public bool IsDisplayedAds = false; //広告を見たか否か

        [NonSerialized]
        public Level ExerciseAmountLevel = Level.Midium; //(値1～3)

        [NonSerialized]
        public Level ExerciseSpeedLevel = Level.Midium;  //(値1～3)

        public enum Level
        {
            Low = 1,
            Midium = 2,
            High = 3
        }

        public const int MaxNbackLevel = 9; //最大レベル
        public const int MinNbackLevel = 2; //最小レベル
        public const int RequireAdsNbackLevel = 5; //広告視聴要求レベル

        public void SetNbackLevel(int level)
        {
            if (level < MinNbackLevel || level > MaxNbackLevel) NbackLevel = MinNbackLevel;

            NbackLevel = level;
        }

        public void SetNextNbackLevel()
        {
            if (NbackLevel == MaxNbackLevel) return;
            NbackLevel++;
        }
        public void SetPrevNbackLevel()
        {
            if (NbackLevel == MinNbackLevel) return;
            NbackLevel--;

        }
    }
}