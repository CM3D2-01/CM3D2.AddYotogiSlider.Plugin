##CM3D2.AddYotogiSlider.Plugin 

夜伽コマンド画面中にF5でGUI表示トグル。  
夜伽中のメイドのステータス・表情等ををスライダー・ボタンで変更する事が可能。  
![GUI](http://i.imgur.com/Sqw8ToS.png "GUI")  



##導入方法

**前提条件** : **UnityInjector** が導入済みであること。  
  
[![ダウンロードボタン][img_download]][master zip]を押してzipファイルをダウンロード。  
zipファイルの中にあるUnityInjectorフォルダをCM3D2フォルダにD&Dすれば導入完了。  



##更新履歴

###0.0.2.3
* 感度スライダー追加。感度値に比例して夜伽コマンド実行時の興奮変動値が増減。
* FaceAnimeパネルにLipsync cancelling追加。有効化でリップシンク停止。
* FaceAnimeパネルで指定できるFace名を追加。
 * "ウインク照れ","にっこり照れ","ダンスあくび","ダンスびっくり","ダンス目あけ","ダンス目つむり"
 * "口開け","追加よだれ","エラー","デフォ","頬０涙０"～"頬２涙３"の+よだれ版
* FaceBlendパネルでも全表情を指定できる様に変更。
* 各パネルタイトルクリックでパネルを折り畳める様に変更。
* AutoAHE表情適用、絶頂数カウントを閾値以上での絶頂時のみに変更。

#####0.0.1.2
* 夜伽画面に入りなおす毎に初期化開始時間が遅くなるバグ修正。

###0.0.1.1
* AutoAHE機能追加。有効化で興奮値に従って瞳が上に移動する様に。絶頂回数で3段階変化。
* AutoBOTE機能追加。有効化で中で出した時にお腹が徐々に膨らんでいく様に。
* 腹スライダー追加。
* FaceAnimeに上書禁止を追加。有効化で夜伽コマンドで表情が上書きされない様に。
* FaceAnimeに適用中の表情名を表示するラベルを追加。
* FaceAnimeに全ての表情のボタンを追加。
* UIの出現の仕方を変更。

###0.0.0.0
* 初版 ([CM3D2.AddModsSlider.Plugin][]から分離)
* 瞳↑移動スライダーを追加
* UIがドラッグで移動できるように


[CM3D2.AddModsSlider.Plugin]: https://github.com/CM3D2-01/CM3D2.AddModsSlider.Plugin "CM3D2-01/CM3D2.AddModsSlider.Plugin"
[master zip]:https://github.com/CM3D2-01/CM3D2.AddYotogiSlider.Plugin/archive/master.zip "master zip"
[img_download]: http://i.imgur.com/byav3Uf.png "ダウンロードボタン"

