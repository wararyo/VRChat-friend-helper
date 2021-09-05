# VRChat Friend Helper

「このフレンド、何の人だっけ…？」を防げるツールです。

これを起動しながらVRChatに入ると、フレンドが同じインスタンスに居るとき、  
そのフレンドにあらかじめ付けたメモを  
コマンドプロンプト、またはVaNiiMenuで確認することができます。

## 導入方法

[Releases](https://github.com/wararyo/VRChat-friend-helper/releases)より最新バージョンをダウンロードします。

### 必要なファイルの準備

ダウンロードしたzipファイルの中にある`friends.csv`を開き、例にならってフレンドの表示名を一人一行で記述します。
UTF-8で保存してください。  
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

### VRChatとVaNiiMenuとVRChat-friend-helperを同時に起動する

適切にファイルを配置することで、
VRChatとVaNiiMenuとVRChat-friend-helperを同時に起動できます。

まず `VRC_friend_helper`フォルダをVaNiiMenu.exeがあるフォルダに移動します。

`VaNiiMenuAndFriendHelper.bat`をVRC_friend_helperフォルダから、VaNiiMenu.exeがあるフォルダに移動します。

```
VaNiiMenu
┣ VRC_friend_helper
┃┣ vrc_friend_helper.exe
┃┗ friends.csv
┣ VaNiiMenu.exe
┗ VaNiiMenuAndFriendHelper.bat
```

移動した`VaNiiMenuAndFriendHelper.bat`を開き、一行目をお使いのPCに合わせて書き換えます。
VRChatをインストールしたフォルダを指定します。

最後にVaNiiMenuAndFriendHelper.batをお好みの方法で起動します。  
(デスクトップにショートカットを作成する、スタートメニューに追加するなど)

## 使い方

VRChat-friend-helperは、VRChatを起動するとログファイルを自動的に検出します。

VaNiiMenuを使用しない場合は、
`vrc_friend_helper.exe`を起動した後にVRChatを起動します。

VaNiiMenuを使用する場合は、先述の`VaNiiMenuAndFriendHelper.bat`を実行するか、  
`vrc_friend_helper.exe`, VRChat, VaNiiMenu の順で起動します。

VaNiiMenuでは、ホーム画面の `- FROM EXTERNAL APPLICATION -` からメモが確認できます。

## 開発とビルド

### Pythonと依存パッケージのインストール

Pythonとpipをインストールします。(WSL2ではなくWindowsにインストールしてください)

下記を実行して必要なパッケージをインストールします。

```
pip install watchdog
pip install python_osc
pip install pyinstaller
```

### ビルド

下記を実行すると、distフォルダの中に実行ファイルが格納されます。

```
pyinstaller .\vrc_friend_helper.py --onefile
```

## TODO

* フレンドが退出したらVaNiiMenuから消す
* ブックマークレットにする
