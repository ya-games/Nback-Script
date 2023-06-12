

## このリポジトリについて
Unityで作成した[Nback手書き計算アプリ](https://play.google.com/store/apps/details?id=com.Y.AGames.N)のC#スクリプトになります。
（Unityプロジェクトには有料アセットが含まれているため、コードのみ公開しています）

Unity version 2021.3.2f

## 使用ライブラリ等
* VContainer(DIコンテナ)
* UniRx
* UniTask
* Unity.Barracuda(onnxをUnityで扱うためのライブラリ)

## 設計について
Model View Presenter（MVP）となるような設計方針としました。
（Modelはアプリケーション本体の部分、ViewはUIを制御する部分、PresenterはModelとViewを繋げる役割）

UniRxのIObservableやReactiveProperty等を使用してModel側から通知を行い、Presenter側で購読することで、Model側からPresenter側を参照しないようにし、参照関係をシンプルにしています。

また、VContainerを使用することで各クラスの依存関係を制御しています。
各シーンで共通して利用するものについては、RootLifetimeScopeにて定義、各シーンで利用するものについては各々LifetimeScopeを定義しています。(Installerフォルダ配下を参照)

メインのゲーム画面については、GameManagerを処理の起点とし、そこからReadyUsecase（準備段階の処理）、InGameUsecase(ゲームのメインロジック)、ResultUsecase(結果画面の表示)を実行するようにしています。


## 手書き処理に関して
手書き数字の画像セットであるMnistを使用しています。
そのまま使用するとあまり精度が良くなかったため、kerasライブラリのImageDataGeneratorを使用して学習データを回転・拡大・縮小・移動させてデータセットを作成し、精度を向上させています。
Unityでonnxファイルを扱うため、Unity.Barracudaを使用しています。



