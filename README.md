# E512TileMap
- Unity2017.2のTileMap機能ではありません
- Unity5.5.0fで作成しました
- E512TileMap.unitypackageをインポートして使います
- font_to_image.py GUIで使う文字テクスチャをttfファイルから生成 python3  
pip install pillow  
font_to_image.py ttf_file_path fontsize8-128  

# 機能
- 広さ制限のないタイルマップ  
広さ制限もできる  
- レイヤー  
- オートタイル  
- ゲームプレイ中のタイル変更  
- シェーダーによるタイルアニメーション  
- タイルマップ生成クラス  
ブロックごとにマップデータがある場合はマップデータを使用し、無い場合はこのクラスを継承したクラスで生成する  
- タイルの明るさ設定  
- タイル用のライト  
１つのグリッド16*16につき8つの光源まで  
- 複数のマップを同時に表示  
マップにGameObjectのレイヤーを設定するなどすればカメラ毎に表示するマップを設定したりカメラ表示レイヤーを変更して切り替えたりできる  
- 複数のカメラで別の位置のマップの表示  
マップにカメラを登録すればカメラの位置のマップが生成される  
- タイルパレット作成用エディタ拡張  
１枚の画像からパレットを作成します  
オートタイルは縦にアニメーションは横に連続して配置したものを使います  
- タイルマップのセーブロード  
autosave editor, pc, android  
autosavedataのload editor, pc, android  
resourcesave editor  
resourcesavedataのload editor, pc, android, webgl  
タイルマップエディタがないため実行中に変更しresourcesaveで終了時に保存します  
- タイルとの当たり判定  
このタイルマップシステム専用の当たり判定  
地形とのあたり判定にUnityのコライダーは使えません  
- オブジェクト同士の当たり判定  
- タイルGUI  
未完成(使えないこともないと思う)
  
# 使って作ったゲーム
- パーティクル素材を使った。デモの炎より豪華なバージョン  
https://ebicochineal.github.io/WebPage/ebifire/
- 第２３回あほげー お題:半分  
http://mogera.jp/gameplay?gid=gm0000003357
- 第３回unity1week お題:積む  
https://unityroom.com/games/ebicochineal_unity1week003
- 第１回unity1week お題:跳ねる  
https://unityroom.com/games/ebicochineal_unity1week001
- Unity 1週間ゲームジャム お題「そろえる」
https://unityroom.com/games/ebicochineal_unity1week202205a
- Unity 1週間ゲームジャム お題「そろえる」
https://unityroom.com/games/ebicochineal_unity1week202205b

# Gif Image
![Gif](https://raw.githubusercontent.com/ebicochineal/Images/master/0.gif)
![Gif](https://raw.githubusercontent.com/ebicochineal/Images/master/12345.gif)


## License
MIT