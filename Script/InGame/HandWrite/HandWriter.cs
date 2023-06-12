using System;
using System.Linq;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using Assets._MyGame.Script.InGame.State;
using Assets._MyGame.Script.Exercises;
using Assets._MyGame.Script.Inference;

namespace Assets._MyGame.Script.InGame.HandWrite
{
    /// <summary>
    /// 手書き処理を行うクラス
    /// </summary>
    public class HandWriter : MonoBehaviour
    {
        //Inspector
        [SerializeField]
        private RectTransform _handWriteArea;

        //DI
        private InGameState _inGameState;
        private ExerciseProvider _exerciseProvider;
        private NumberInference _numberInference;

        //Field
        private Graphic _graphic;
        private Texture2D _texture;
        private const int _onnxInputWidth = NumberInference.OnnxInputWidth;
        private const int _onnxInputHeight = NumberInference.OnnxInputHeight;
        private const int _handWriteTextureWidth = 400;
        private const int _handWriteTextureHeight = 400;
        private const int _lineWidth = 12;
        private const int _invalidPixelThreshold = 8;//このピクセル数以下の場合には手書きの判定を行わない

        private readonly Color[] _buffer = new Color[_handWriteTextureWidth * _handWriteTextureHeight];
        private readonly float[] _onnxInput = new float[_onnxInputWidth * _onnxInputHeight];

        private readonly Subject<AnswerData> _onCorrectAnswer = new();
        private readonly Subject<Unit> _onIncorrectAnswer = new();
        private readonly Subject<int> _onShowPlayerAnswer = new();



        [Inject]
        public void Constructor(
            InGameState inGameState,
            ExerciseProvider exerciseProvider,
            NumberInference numberInference
            )
        {
            _inGameState = inGameState;
            _exerciseProvider = exerciseProvider;
            _numberInference = numberInference;

            //ストリームソースのDispose
            _onCorrectAnswer.AddTo(this);
            _onIncorrectAnswer.AddTo(this);
            _onShowPlayerAnswer.AddTo(this);
        }

        public IObservable<AnswerData> OnCorrectAnswer => _onCorrectAnswer;
        public IObservable<Unit> OnIncorrectAnswer => _onIncorrectAnswer;
        public IObservable<int> OnShowPlayerAnswer => _onShowPlayerAnswer;




        private void Start()
        {
            _graphic = _handWriteArea.GetComponent<Graphic>();

            _texture = new Texture2D(_handWriteTextureWidth, _handWriteTextureHeight);
            ClearBuffer();

            var image = _handWriteArea.gameObject.GetComponent<Image>();
            image.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), Vector2.zero);

            SetupMouseEvent();
        }

        private void OnDestroy()
        {
            Destroy(_texture);
            _numberInference?.Dispose();
        }

        /// <summary>
        /// マウスイベントの購読
        /// </summary>
        void SetupMouseEvent()
        {
            var et = _handWriteArea.gameObject.AddComponent<ObservableEventTrigger>();

            bool touching = false;
            Vector2 prevPoint = Vector2.zero;

            this.UpdateAsObservable()
                .SkipUntil(et.OnPointerDownAsObservable())
                .TakeUntil(et.OnPointerUpAsObservable())
                .DoOnCompleted(() =>
                {
                    touching = false; //マウスクリックがUpになった時にフラグを変更
                })
                .RepeatUntilDestroy(this)
                .Where(x => _inGameState.IsState(InGameStateType.CAN_ANSWER) || _inGameState.IsState(InGameStateType.ANSWERED)) //手書き可能なステータスのみ
                .Select((_, i) => new { Pos = Input.mousePosition, Index = i })
                .Subscribe(x =>
                {
                    var pos = GetClickedTexturePixels();

                    if (touching)
                    {
                        DrawLine(prevPoint, pos); //クリック中は前回位置から現在位置まで線を引く
                    }
                    else
                    {
                        Draw(Vector2Int.RoundToInt(pos)); //初回は点を打つ
                    }
                    touching = true;
                    prevPoint = pos;

                    _texture.SetPixels(_buffer);
                    _texture.Apply();
                }).AddTo(this);



            //マウスアップのタイミングで判定を行うストリームを購読        
            et.OnPointerUpAsObservable()
               .Throttle(TimeSpan.FromMilliseconds(500)) //0.5秒後に判定(4とか5の数字を正しく判定するため）
               .Where(x => _inGameState.IsState(InGameStateType.CAN_ANSWER))
               .Subscribe(x =>
               {
                   var playerAnswer = GetPlayerAnswer();

                   //回答した数字を表示
                   _onShowPlayerAnswer.OnNext(playerAnswer);

                   //正解の場合は〇表示の通知を行う。（不正解の場合は制限時間まで再度回答可とする）
                   if (playerAnswer.ToString() == _exerciseProvider.CurrentBottomExercise.Answer)
                   {
                       _exerciseProvider.CurrentBottomExercise.IsCorrect = true;

                       //回答を通知(InGameUsecase側で受け取る）
                       var answerData = new AnswerData(_exerciseProvider.CurrentBottomExercise.No, playerAnswer);
                       _onCorrectAnswer.OnNext(answerData);
                   }
                   else
                   {
                       //不正解の場合
                       _onIncorrectAnswer.OnNext(default);
                   }
               })
               .AddTo(this);
        }

        /// <summary>
        /// マウスポジションから、画像のテクスチャーピクセル座標を求める
        /// 【Unity】【uGUI】RectTransformUtilityでスクリーン座標をUIのローカル座標やワールド座標に変換する - LIGHT11 https://light11.hatenadiary.com/entry/2019/04/16/003642
        /// </summary>
        Vector2 GetClickedTexturePixels()
        {
            Rect r = _handWriteArea.rect;
            Vector2 localPoint;

            var camera = _graphic.canvas.worldCamera;
                        
            if (_graphic.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                camera = null;
            }

            //クリック時の座標はスクリーン座標のため、画像のローカル座標を取得する
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_handWriteArea, Input.mousePosition, camera, out localPoint);


            //ローカル座標から、テクスチャーのピクセル座標を求める
            //左下を(0,0)として計算する
            int px = Mathf.Clamp((int)(((localPoint.x - r.x) * _texture.width) / r.width), 0, _texture.width);
            int py = Mathf.Clamp((int)(((localPoint.y - r.y) * _texture.height) / r.height), 0, _texture.height);

            return new Vector2(px, py);
        }
        /// <summary>
        /// 前回の位置から現在の位置まで線を引く
        /// Unity で2Dお絵かきアプリを作る - たるこすの日記 https://tarukosu.hatenablog.com/entry/2017/02/12/203526
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        private void DrawLine(Vector2 p, Vector2 q)
        {
            var lerpNum = 10;
            for (int i = 0; i < lerpNum + 1; i++)
            {
                var r = Vector2.Lerp(p, q, i * (1.0f / lerpNum));
                Draw(Vector2Int.RoundToInt(r));
            }
        }


        private void Draw(Vector2Int p)
        {

            var r = _lineWidth; //線の半径

            for (var width = 0; width < r; width++)
            {
                for (var height = 0; height < r; height++)
                {
                    if ((Mathf.Pow(width, 2) + Mathf.Pow(height, 2)) < Mathf.Pow(r, 2))
                    {
                        //円の(1/4)ずつセット
                        SetBuffer(p.x + width, p.y + height);
                        SetBuffer(p.x - width, p.y + height);
                        SetBuffer(p.x + width, p.y - height);
                        SetBuffer(p.x - width, p.y - height);
                    }

                }
            }
        }

        private void SetBuffer(int x, int y)
        {
            // テクスチャの座標は左下を(0,0)、右上がmaxサイズになる
            //マイナス座標とmaxサイズを超える場合はreturn
            if (x < 0 || _handWriteTextureWidth <= x || y < 0 || _handWriteTextureHeight <= y)
            {
                return;
            }

            _buffer.SetValue(Color.black, x + _handWriteTextureWidth * y);
        }

        //全バッファーを白で塗りつぶしてクリア
        public void ClearBuffer()
        {
            for (int i = 0; i < _buffer.Length; i++)
            {
                _buffer[i] = Color.white;
            }

            _texture.SetPixels(_buffer);
            _texture.Apply();
        }

        /// <summary>
        /// 手書きで回答した答えからint型の数を返す
        /// </summary>
        /// <returns>
        /// -1:回答なし
        /// 0以上:Playerの回答
        /// </returns>
        private int GetPlayerAnswer()
        {

            //手書きしたテクスチャを複製する
            var distTexture = new Texture2D(_texture.width, _texture.height);
            Graphics.CopyTexture(_texture, distTexture);
            //バイリニア補完を行い、28*28のテクスチャに変換する
            TextureScaler.Bilinear(distTexture, _onnxInputWidth, _onnxInputHeight);

            var texpixels = distTexture.GetPixels();

            //GetPixels()で取得した配列は、テクスチャの左下を(0,0)として右方向にカウントアップしていく
            //一方、Mnistで判定を行うデータは、左上を(0,0)として右方向にカウントアップしていくため、配列の並び順を変更する
            //（例）3*3なら下記のような順番に変更する
            //{ 1, 2, 3, 4, 5, 6, 7, 8, 9}　→　{ 7, 8, 9, 4, 5, 6, 1, 2, 3}
                        
            //Linqで書く場合には、下記のように28個で区切る→それらのグループを逆順→平坦化            
            //var chunksSize = _onnxInputWidth;
            //var chunks = texpixels.Select((v, i) => new { v, i })
            //                        .GroupBy(x => x.i / chunksSize)
            //                        .Select(g => g.Select(x => x.v));
            //texpixels = chunks.Reverse().SelectMany(x => x).ToArray();            

            int index = 0;
            int filledPixelCount = 0;//塗りつぶされたピクセル数
            for (int i = 1; i <= _onnxInputWidth; i++)
            {
                for (int j = 0; j < _onnxInputHeight; j++)
                {
                    var color = texpixels[_onnxInput.Length - (i * _onnxInputHeight) + j];

                    //input[index] = 1- Mathf.Floor(color.r);
                    _onnxInput[index] = 1 - Mathf.Ceil(color.r); ////白が1,黒が0なので、反転するため1から引く(Ceilで切り上げてから）
                    if (_onnxInput[index] >= 1.0) filledPixelCount++;

                    index++;
                }
            }

            Destroy(distTexture);

            //塗られたピクセル数が少ない場合には、推論せずにreturn(1ドットだけ塗った場合など）
            if (filledPixelCount <= _invalidPixelThreshold) return -1; 

            return _numberInference.Inference(_onnxInput); ;
        }

    }
}
