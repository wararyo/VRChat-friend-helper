# VRChat Friend Helper

「このフレンド、何の人だっけ…？」を防げるツールです。

これを起動しながらVRChatに入ると、フレンドが同じインスタンスに居るとき、  
そのフレンドにあらかじめ付けたメモを  
コマンドプロンプト、またはVaNiiMenuで確認することができます。

## 導入方法

### Pythonと依存パッケージのインストール

Pythonとpipをインストールします。(WSL2では動作しなかったため、Windowsにインストールしてください)

下記を実行して必要な依存パッケージをインストールします。

```
pip install watchdog
pip install python_osc
```

### 必要なファイルの準備

次にこのリポジトリをダウンロード、またはクローンします。

保存したリポジトリの中にある`friends.csv` を開き、例にならってフレンドの表示名を記述します。UTF-8で保存してください。  
CSV形式ですが、`#`から始まる行はコメントとして扱われます。

初回の作成はChromeでVRChatを開いてREPLで下記を実行すると楽です。  
TODO: ブックマークレットにした方が楽そう

``` javascript
let friendsHTML = [...document.querySelectorAll(".friend-container .user-info h6 a")].map(x => x.innerText).join(", <br>")
document.write(friendsHTML)
```

### VaNiiMenuの設定

VaNiiMenuを使用する場合は、VaNiiMenuの`config/Communication.json` を開き、下記のように書き換えます。

``` json
{"OSCReceive":true,"jsonVer":1}
```

## 使い方

コマンドプロンプトやPowerShellで、保存したリポジトリのフォルダに移動します。  
下記のように`vrc_friend_helper.py`を実行するだけで、自動的にログファイルを検出します。

```
python ./vrc_friend_helper.py
```

VaNiiMenuでは、ホーム画面の `- FROM EXTERNAL APPLICARTION -` からメモが確認できます。

## TODO

フレンドが退出したらVaNiiMenuから消す
PyInstallerで実行ファイルにする
