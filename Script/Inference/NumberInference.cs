using Unity.Barracuda;
using System;


namespace Assets._MyGame.Script.Inference
{  
    /// <summary>
    /// 数字の推論クラス
    /// </summary>
    public class NumberInference : IDisposable
    {
        private readonly IWorker _worker;
        public const int OnnxInputWidth = 28;
        public const int OnnxInputHeight = 28;
        

        public NumberInference(NNModel nnModel)
        {
            var runtimeModel = ModelLoader.Load(nnModel);
            _worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, runtimeModel);
        }

        
        //Mnistは28x28のfloat値(0~1)のinputで推論できる，左上が原点で右下に向かう座標系
        public int Inference(float[] inputFloats)
        {            
            var scores = InferenceOnnx(inputFloats);


            //最大のIndexを求める．Indexが推論した数字
            var maxScore = float.MinValue;
            int maxIndex = 0;
            for (int i = 0; i < scores.Length; i++)
            {
                float score = scores[i];
                if (maxScore < score)
                {
                    maxScore = score;
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        private float[] InferenceOnnx(float[] input)
        {
            float[] output;
            using (var inputTensor = new Tensor(1, OnnxInputWidth, OnnxInputHeight, 1, input)) {
                _worker.Execute(inputTensor);
                using (var outputTensor = _worker.PeekOutput())
                {
                    output = outputTensor.ToReadOnlyArray();
                }                
            }
            return output;
        }

        public void Dispose()
        {
            _worker?.Dispose();
        }        
    }

}
