using System.Collections.Generic;
using System.Linq;

namespace Assets._MyGame.Script.Exercises
{
    /// <summary>
    /// 問題集の結果クラス
    /// </summary>
    public class ExerciseResult
    {
        public const int CorrectThreshold = 65;//合格の閾値(%)

        public IList<bool> ResultList { get; }
        
        public int ExerciseCount => ResultList.Count; //問題数

        public int CorrectPercent => (int)((float)ResultList.Count(x => x==true) / ResultList.Count * 100); //正解率(%)

        public bool IsPass => CorrectPercent >= CorrectThreshold;　//合格か否か

        public ExerciseResult(IList<Exercise>list)
        {
            ResultList = list.Select(x => x.IsCorrect).ToList();
        }
    }
}

