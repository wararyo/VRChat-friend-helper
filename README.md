# VRChat Friend Helper

「このフレンド、何の人だっけ…？」を防げるツールです。

これを起動しながらVRChatをすると、
フレンドが同じインスタンスに入ったとき、
そのフレンドにあらかじめ付けたメモをVaNiiMenuに表示することができます。

## 導入方法

Pythonとpipをインストールします。(WSLおよびWSL2では動作しなかったため、Windowsにインストールしてください)

下記を実行します。

```
pip install watchdog
pip install python_osc
```

`friends.csv` というファイルを作成します。UTF-8で保存してください。
CSV形式ですが、`#`から始まる行はコメントとして扱われます。

初回のフレンドリストはChromeでVRChatを開いてREPLで下記を実行すると楽です。

``` javascript
let friendsHTML = [...document.querySelectorAll(".friend-container .user-info h6 a")].map(x => x.innerText).join(", <br>")
document.write(friendsHTML)
```

TODO: ブックマークレットにした方が楽そう

VaNiiMenuを使用する場合は、VaNiiMenuの`config/Communication.json` を開き、下記のように書き換えます。

``` json
{"OSCReceive":true,"jsonVer":1}
```

## 使い方

下記のように`vrc_friend_helper.py`を実行するだけで、自動的にログファイルを検出します。

```
python ./vrc_friend_helper.py
```

VaNiiMenuでは、ホーム画面の `- FROM EXTERNAL APPLICARTION -` からメモが確認できます。
