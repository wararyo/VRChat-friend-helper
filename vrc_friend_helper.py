#!/usr/bin/env python
from __future__ import print_function

import sys
import os
import time
import re
import csv
import subprocess
from watchdog.observers import Observer
from watchdog.events import PatternMatchingEventHandler

# 初回のフレンドリストはChromeでVRChatを開いてREPLで下記を実行すると取得できます
# let friendsHTML = [...document.querySelectorAll(".friend-container .user-info h6 a")].map(x => x.innerText).join(", <br>")
# document.write(friendsHTML)

# [User]の部分をユーザー名に書き換えてください
LOG_DIRECTORY = "C:/Users/wararyo/AppData/LocalLow/VRChat/VRChat/"
LOG_PREFIX = "output_log_"
LOG_EXTENSION = "txt"

FRIENDS_FILE_NAME = "friends.csv"
FRIENDS_CSV_HEADER = ["UserName", "Description"]

friends = []

def find(func, arr):
    rs = list(filter(func, arr))
    if len(rs) == 0: return None
    return rs[0]

class MyHandler(PatternMatchingEventHandler):
    log_file = None
    log_file_name = ""
    def __init__(self, patterns):
        super(MyHandler, self).__init__(patterns=patterns)

    def on_modified(self, event):
        file_name = os.path.basename(event.src_path)

        # 新たなログファイルを検知したときはそのログファイルを開く
        if file_name != self.log_file_name:
            self.log_file_name = file_name
            print("New log file detected:",self.log_file_name)
            if self.log_file != None:
                self.log_file.close()
            self.log_file = open(event.src_path, 'r', encoding="utf-8")

        # ログファイルの新たに追記された部分を読んでいく
        line = self.log_file.readline()
        while line:

            # 部屋の移動
            match=re.search('([0-9\.]+ [0-9:]+).+Joining or Creating Room: (.+)',line)
            if match != None:
                print("\n" + match.group(1) + " World: " + match.group(2))

            # プレイヤーのロード
            match=re.search('([0-9\.]+ [0-9:]+).+\[Behaviour\] Initialized PlayerAPI "(.+)" is remote',line)
            if match != None:
                user_name = match.group(2)
                friend = find(lambda x: x["UserName"] == user_name, friends)
                if friend != None:
                    print(match.group(1),"User:",user_name,",",friend["Description"])

            # プレイヤーの退出
            # match=re.search('([0-9\.]+ [0-9:]+).+\[Behaviour\] OnPlayerLeft "(.+)"',line)
                # if match != None:

            # フレンド申請の承認
            match=re.search('([0-9\.]+ [0-9:]+).+AcceptNotification for notification:.+ username:([^,]+),.*type: friendRequest.*',line)
            if match != None:
                user_name = match.group(2)
                print(match.group(1),"FriendRequest Accepted:",user_name)
                # 新たなフレンドをフレンド一覧に追加
                if find(lambda x: x["UserName"] == user_name, friends) == None:
                    with open(FRIENDS_FILE_NAME, 'a', encoding="utf-8") as f:
                        print(user_name+",", file=f)
                    # 念のためフレンド一覧を再読み込み
                    load_friends()

            line = self.log_file.readline()

    def on_stop(self):
        if self.log_file != None:
            self.log_file.close()

def watch():
    event_handler = MyHandler([LOG_PREFIX+"*"+LOG_EXTENSION])
    observer = Observer()
    observer.schedule(event_handler, LOG_DIRECTORY, recursive=False)
    observer.start()
    print("Watching logs in {}".format(LOG_DIRECTORY))
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        event_handler.on_stop()
        observer.stop()
    observer.join()

def load_friends():
    global friends
    with open(FRIENDS_FILE_NAME, 'r', encoding="utf-8") as f:
        reader = csv.DictReader(filter(lambda row: row[0]!='#', f), FRIENDS_CSV_HEADER)
        friends = [row for row in reader]
    print("{} friends has been loaded.".format(len(friends)))

if __name__ == "__main__":
    # フレンド一覧をCSVファイルから読み込み
    load_friends()
    # ログファイルの監視を開始
    watch()
