#!/usr/bin/env python
from __future__ import print_function

import sys
import os
import time
import re
import csv
import subprocess
import datetime
from watchdog.observers import Observer
from watchdog.events import PatternMatchingEventHandler
from pythonosc import udp_client
from pythonosc.osc_message_builder import OscMessageBuilder

# 初回のフレンドリストはChromeでVRChatを開いてREPLで下記を実行すると取得できます
# let friendsHTML = [...document.querySelectorAll(".friend-container .user-info h6 a")].map(x => x.innerText).join(", <br>")
# document.write(friendsHTML)

LOG_DIRECTORY = os.path.expandvars(r"%appdata%/../LocalLow/VRChat/VRChat/")
LOG_PREFIX = "output_log_"
LOG_EXTENSION = "txt"

FRIENDS_FILE_NAME = "friends.csv"
FRIENDS_CSV_HEADER = ["UserName", "Description"]

OSC_IP = '127.0.0.1'
OSC_PORT = 39972

friends = []
display = None

# listから条件を満たす項目を探す
def find(func, arr):
    rs = list(filter(func, arr))
    if len(rs) == 0: return None
    return rs[0]

class VaNiiMenuDisplay:
    osc_client = None
    osc_contents = []

    def __init__(self, ip, port):
        self.osc_client = udp_client.UDPClient(ip, port)

    def addContent(self, text):
        self.osc_contents.append(text)

    def clearContent(self):
        self.osc_contents.clear()

    def update(self):
        msg = OscMessageBuilder(address='/VaNiiMenu/HomeInfo')
        msg.add_arg("\n".join(self.osc_contents))
        m = msg.build()

        self.osc_client.send(m)

# VRChatが出すログを監視する
class LogEventHandler(PatternMatchingEventHandler):
    log_file = None
    log_file_name = ""
    def __init__(self, patterns):
        super(LogEventHandler, self).__init__(patterns=patterns)

    def on_created(self, event):
        file_name = os.path.basename(event.src_path)

        # 新たなログファイルを検知したときはそのログファイルを開く
        if file_name != self.log_file_name:
            self.log_file_name = file_name
            print("New log file created:",self.log_file_name)
            if self.log_file != None:
                self.log_file.close()
            self.log_file = open(event.src_path, 'r', encoding="utf-8")

    def on_modified(self, event):
        file_name = os.path.basename(event.src_path)

        # 新たなログファイルを検知したときはそのログファイルを開く
        if file_name != self.log_file_name:
            self.log_file_name = file_name
            print("New log file modified:",self.log_file_name)
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
                display.clearContent()
                # 念のためフレンド一覧を再読み込み
                load_friends()

            # プレイヤーの参加
            match=re.search('([0-9\.]+ [0-9:]+).+\[Behaviour\] Initialized PlayerAPI "(.+)" is remote',line)
            if match != None:
                user_name = match.group(2)
                friend = find(lambda x: x["UserName"] == user_name, friends)
                if friend != None:
                    print(match.group(1),"User:",user_name,",",friend["Description"])
                    display.addContent(user_name + ": " + friend["Description"])

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

            line = self.log_file.readline()
        
        # VaNiiMenuを更新
        display.update()

    def on_stop(self):
        if self.log_file != None:
            self.log_file.close()

# ファイルとコンソール両方に出力するロガー
class Logger(object):
    def __init__(self):
        self.terminal = sys.stdout
        now = datetime.datetime.now()
        self.log = open(now.strftime("%Y-%m-%d_%H-%M-%S.log"), "a", encoding="utf-8")

    def write(self, message):
        self.terminal.write(message)
        self.log.write(message)

    def flush(self):
        pass

    def close(self):
        self.log.close()

def watch():
    event_handler = LogEventHandler([LOG_PREFIX+"*"+LOG_EXTENSION])
    observer = Observer()
    observer.schedule(event_handler, LOG_DIRECTORY, recursive=False)
    observer.start()
    print("Watching logs in {}".format(LOG_DIRECTORY))
    try:
        while True:
            time.sleep(5)
            # Watchdogだけだとなぜか変更が検知されないため、定期的にログファイルを開く
            if event_handler.log_file_name != "":
                open(os.path.join(LOG_DIRECTORY,event_handler.log_file_name), 'r', encoding="utf-8").close()
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
    sys.stdout = Logger()
    display = VaNiiMenuDisplay(OSC_IP, OSC_PORT)
    # フレンド一覧をCSVファイルから読み込み
    load_friends()
    # ログファイルの監視を開始
    watch()
    sys.stdout.close()
