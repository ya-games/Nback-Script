
namespace Assets._MyGame.Script.InGame.HandWrite
{
    /// <summary>
    /// 回答データ
    /// </summary>
    public struct AnswerData
    {
        public int No { get; }
        public int PlayerAnswer { get; }
        public AnswerData(int no,int answer)
        {
            No = no;
            PlayerAnswer = answer;
        }
    }
}
