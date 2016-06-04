##CM3D2.AddYotogiSlider.Plugin (ANALKUPA 拡張 Ver.)

夜伽コマンド画面中にF5でGUI表示トグル。  
夜伽中のメイドのステータス・表情等ををスライダー・ボタンで変更する事が可能。  
![GUI](http://i.imgur.com/KNSFUPR.png "GUI")  



##導入方法

**前提条件** : **UnityInjector** が導入済みであること。  
  
[![ダウンロードボタン][img_download]][master zip]を押してzipファイルをダウンロード。  
zipファイルの中にあるUnityInjectorフォルダをCM3D2フォルダにD&Dすれば導入完了。  

ANALKUPA 機能を使うには **'analkupa' 対応 body が必要です。**
対応 body がなくてもそれ以外の機能は動作します。
また、labiakupa, suji, clitoris に対応しているbodyではクリが興奮によって変化します。


##更新履歴

### 0.1.0.1
* 興奮に反応してクリがアニメーションするようにした。
* 拡張度スライダーを追加。拡張度スライダーの倍率はkupa, analkupaに反映される
* labiakupa, suji, clitoris に対応するスライダーを追加。

### 0.0.4.7 + v20151214
* `[AutoKUPA_Anal] Value_*` の読み込みを修正。
  (`[AutoKUPA]` を参照していた。)

### 0.0.4.7 + v20151212
* GUIオープンキー(F5)を ini で指定可能に。

* Toggle 値保存の修正。

* AutoKUPA で、夜伽開始時の初期値を指定可能に。

* スキル判定・閉じる判定を修正。

* AutoKUPA アニメーションの開始点を現在値からとする。

* 夜伽スキル開始時に KUPA 状態を適用。(バイブ装着スキル等)

* 夜伽スキル開始時に orgasm シェイプをリセット。
  (orgasm アニメーション中に次スキル開始するとずれたままになっていた。)

### 0.0.4.7 + v20151207
* AutoKUPA に待機モーションを追加。
  ini ファイルの `[AutoKUPA] WaitingValue` で開き値幅を設定。

* Toggle 値を保存。
  (AutoAHE, AutoBOTE, AutoKUPA, OrgasmConvulsion のみ)

* スキル判定を修正。
  ("シックスナイン", "ポーズ維持プレイ", "4P", "～ Chu-B" などに対応)

* `[AutoKUPA] IncrementPerOrgasm` のバグを修正。
  * 常に `NormalMax` に固定されていた。
  * 次のスキル開始時に、コマンドを選択するまで適用されていなかった。

### 0.0.4.7 + v20151203
* AutoKUPA 'Enabled' 時、対応 body の 'analkupa' シェイプキーを自動操作。

* AutoKUPA パネルにスライダーを追加。
 * "前" スライダー  :  KUPA 値を操作します。 *オリジナルでは "値" スライダー。*
 * "後" スライダー  :  ANALKUPA 値を操作します。 **非対応 body の場合表示されません。**

* AddYotogiSlider.ini ファイルに `[AutoKUPA_Anal]` 項目を追加。
  詳細はファイル内のコメントを参照してください。

###0.0.4.7
* AutoAHE/BOTE/KUPAの各種数値をiniファイルで設定可能に変更。(./Config/AddYotogiSlider.ini)
* AHE絶頂回数に従ってKUPA値が一定以上に下がらない様に変更。(改造スレ5>>446)
* バイブ責め系スキル時にAutoKUPAが機能していなかったバグ修正。
* アニメ再生中にコマンド実行時、kupaキー操作がharaキー操作になっていたバグ修正。


#####0.0.3.6
* 'kupa','orgasm'対応bodyで無い場合に該当項目のUIを表示しない内容に変更。
* 'kupa','orgasm'対応bodyで無い場合にエラーメッセージが出るバグ修正。

#####0.0.3.5
* 男が表示されない夜伽スキルで初期化が完了しないバグ修正。
* 男が複数される夜伽スキルで1人目の速度しか変更されないバグ修正。
* 感度の値の計算が正しく行われてなかったバグ修正。

###0.0.3.4
* 背景セレクタ追加。
* 速度スライダー追加。モーション速度を操作可能。
* AutoKUPAパネル・スライダー追加。有効化で挿入時にシェイプキー'kupa'を操作。※1
* AutoAHEパネルにOrgasm convulsion追加。有効化でAHE絶頂時にシェイプキー'orgasm'を操作。※1
* AutoAHE絶頂時にモーション速度が変わる様に変更。
* スライダーピン有効かつパネル無効時にメイドの値を書き替えていたバグ修正。

※1:対応するシェイプキーを持つbodyが必要です。各自で用意して下さい。


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



##注意書き

個人で楽しむ為の非公式Modです。  
転載・再配布・改変・改変物配布等はKISSに迷惑のかからぬ様、  
各自の判断・責任の下で行って下さい。  

改変版は複数の開発者のバトンリレーで製作されたものです。
改変版について、オリジナルの作者さまに問い合わせ等を行わないでください。
改変版のソース差分の利用は自由です。

Based on: CM3D2.AddYotogiSlider Ver.0.0.4.7
[CM3D2.AddYotogiSlider.Plugin]: https://github.com/CM3D2-01/CM3D2.AddYotogiSlider.Plugin

[CM3D2.AddModsSlider.Plugin]: https://github.com/CM3D2-01/CM3D2.AddModsSlider.Plugin "CM3D2-01/CM3D2.AddModsSlider.Plugin"
[master zip]:https://github.com/CM3D2-01/CM3D2.AddYotogiSlider.Plugin/archive/master.zip "master zip"
[img_download]: http://i.imgur.com/byav3Uf.png "ダウンロードボタン"

「MODはKISSサポート対象外です。」
「MODを利用するに当たり、問題が発生しても製作者・KISSは一切の責任を負いかねます。」
「カスタムメイド3D2を購入されている方のみが利用できます。」
「カスタムメイド3D2上で表示する目的以外の利用は禁止します。」
