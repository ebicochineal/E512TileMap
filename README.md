# E512TileMap
- Unity5.5.0fで作成
- E512TileMap.unitypackage

# 機能
- 広さ制限のないタイルマップ  
広さ制限もできる  
- レイヤー  
- オートタイル  
- ゲームプレイ中のタイル変更  
- シェーダーによるタイルアニメーション  
- タイルの明るさ設定  
- タイル用のライト １つのグリッド  
１６＊１６につき８つの光源まで  
- 複数のマップを同時に表示  
マップにGameObjectのレイヤーを設定するなどすればカメラ別に表示するマップを設定したりカメラ表示レイヤーを変更して切り替えたりできる  
- 複数のカメラで別の位置のマップの表示  
マップにカメラを登録すればカメラの位置のマップが生成される  
- タイルパレット作成用エディタ拡張  
１枚の画像からパレットを作成します  
オートタイルは縦にアニメーションは横に連続して配置したものを使います  
- タイルマップのセーブロード  
セーブロードeditor,pc,android ロードwebgl  
タイルマップエディタがないため実行中に変更しresource saveで終了時に保存します  
- タイルとの当たり判定  
地形とのあたり判定にUnityのコライダーは使えません  
- オブジェクト同士の当たり判定  
- タイルGUI  
未完成
  
![Gif](https://raw.githubusercontent.com/ebicochineal/Images/master/0.gif)
![Gif](https://raw.githubusercontent.com/ebicochineal/Images/master/1.gif)
![Gif](https://raw.githubusercontent.com/ebicochineal/Images/master/2.gif)
![Gif](https://raw.githubusercontent.com/ebicochineal/Images/master/3.gif)
![Gif](https://raw.githubusercontent.com/ebicochineal/Images/master/4.gif)
![Gif](https://raw.githubusercontent.com/ebicochineal/Images/master/5.gif)
![Gif](https://raw.githubusercontent.com/ebicochineal/Images/master/6.gif)