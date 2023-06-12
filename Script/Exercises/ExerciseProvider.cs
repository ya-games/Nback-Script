using System.Collections.Generic;
using System.Linq;
using Assets._MyGame.Script.GameInformation;

namespace Assets._MyGame.Script.Exercises
{
    /// <summary>
    /// 問題の供給を行うクラス
    /// </summary>
    public class ExerciseProvider
    {
        private GameInfoSO _gameInfoSO;

        public List<Exercise> ExerciseList { get; private set; }

        public int ExcersiseCount { get; private set; } = 20; //問題数
        private int topIndex = -1;//上段に表示する問題のIndex
        private int bottomIndex = -1;//下段に表示する問題のIndex

        public Exercise CurrentTopExercise => ExerciseList.ElementAt(topIndex);
        public Exercise CurrentBottomExercise => ExerciseList.ElementAt(bottomIndex);

        public bool Done { get; private set; }

        public ExerciseProvider(GameInfoSO gameInfoSO)
        {
            _gameInfoSO = gameInfoSO;
            Initialize();
        }

        void Initialize()
        {
            //問題数を設定
            switch (_gameInfoSO.NbackLevel)
            {
                case 2:
                    ExcersiseCount = 20;
                    break;
                case 3:
                    ExcersiseCount = 20;
                    break;
                case 4:
                    ExcersiseCount = 30;
                    break;
                case 5:
                    ExcersiseCount = 30;
                    break;
                case 6:
                    ExcersiseCount = 35;
                    break;
                case 7:
                    ExcersiseCount = 35;
                    break;
                case 8:
                    ExcersiseCount = 40;
                    break;
                case 9:
                    ExcersiseCount = 40;
                    break;
                default:
                    ExcersiseCount = 20;
                    break;
            }
            //問題数レベルによって増減
            switch (_gameInfoSO.ExerciseAmountLevel)
            {
                case GameInfoSO.Level.Low:
                    ExcersiseCount -= 10;
                    break;
                case GameInfoSO.Level.High:
                    ExcersiseCount += 10;
                    break;
            }

            ExerciseList = new List<Exercise>(ExcersiseCount);

            foreach (var i in Enumerable.Range(1, ExcersiseCount))
            {
                ExerciseList.Add(new Exercise(i));
            }

        }


        /// <summary>
        /// 次の問題へ移動する(初期値は-1なので開始時に必ず呼ぶ)
        /// </summary>
        /// <returns></returns>
        public bool MoveNextTopIndex()
        {
            //index範囲が要素数を超える場合はfalse
            if (topIndex + 1 >= ExcersiseCount)
            {
                return false;
            }
            topIndex++;
            return true;
        }

        /// <summary>
        /// 次の問題へ移動する(初期値は-1なので開始時に必ず呼ぶ)
        /// </summary>
        /// <returns></returns>
        public bool MoveNextBottomIndex()
        {
            //index範囲が要素数を超える場合はfalse
            if (bottomIndex + 1 >= ExcersiseCount)
            {
                Done = true;
                return false;
            }
            bottomIndex++;
            return true;
        }
    }
}
