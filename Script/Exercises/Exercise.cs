
using Rand = UnityEngine.Random;

namespace Assets._MyGame.Script.Exercises
{
    /// <summary>
    /// 問題1つを表すクラス
    /// </summary>
    public class Exercise
    {
        public int No { get; private set; }
        public string Formula { get; private set; }
        public string Answer { get; set; }        
        public bool IsCorrect { get; set; }

        public Exercise(int no)
        {
            this.No = no;
                        
            var answerNum = Rand.Range(0, 10);
            var leftNum = Rand.Range(1, 10);
            var rightNum = answerNum - leftNum;

            var rightStr = rightNum >= 0 ? $"+{rightNum.ToString()}" : rightNum.ToString();
            this.Formula = $"{leftNum.ToString()}{rightStr}=";
            //this.Formula  =string.Format("{0}{1}=", leftNum.ToString(), rightNum.ToString("+#;-#;"));
            this.Answer = answerNum.ToString();
            this.IsCorrect = false;
        }

    }
}
