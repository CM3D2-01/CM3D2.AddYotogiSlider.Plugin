## CM3D2.AddYotogiSlider.Plugin (ANALKUPA 拡張 Ver.)

オリジナルの CM3D2.AddYotogiSlider.Plugin に 'ANALKUPA' 機能を追加。

Based on: CM3D2.AddYotogiSlider Ver.0.0.4.7

[CM3D2.AddYotogiSlider.Plugin]: https://github.com/CM3D2-01/CM3D2.AddYotogiSlider.Plugin



## 内容物

* README-analkupa.txt

* original-README.md
  **オリジナルの README。必ず参照してください。**

* CM3D2.AddYotogiSlider.Plugin.cs
  改変版のソースファイル。

* UnityInjector/CM3D2.AddYotogiSlider.Plugin.dll
  改変版のバイナリ。

* UnityInjector/Config/AddYotogiSlider.ini
  改変版の設定ファイル。



## 導入方法

オリジナルと同様に導入してください。
UnityInjector フォルダを CM3D2 フォルダにコピーすれば完了です。

**UnityInjector** が導入済みであること。

ANALKUPA 機能を使うには **'analkupa' 対応 body が必要です。**
対応 body がなくてもそれ以外の機能は動作します。



## オリジナルとの差異

### 0.0.4.7 + v20151211
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



## 注意書き

個人で楽しむ為の非公式Modです。
転載・再配布・改変・改変物配布等はKISSに迷惑のかからぬ様、
各自の判断・責任の下で行って下さい。

改変版について、オリジナルの作者さまに問い合わせ等を行わないでください。

改変版のソース差分の利用は自由です。

