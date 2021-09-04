#!/usr/bin/env python
from __future__ import print_function

import sys
import os
import time
import re
import subprocess
from watchdog.observers import Observer
from watchdog.events import PatternMatchingEventHandler

# [User]の部分をユーザー名に書き換えてください
LOG_DIRECTORY = "C:/Users/wararyo/AppData/LocalLow/VRChat/VRChat/"
LOG_PREFIX = "output_log_"
LOG_EXTENSION = "txt"

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

            match=re.search('([0-9\.]+ [0-9:]+).+Joining or Creating Room: (.+)',line)
            if match != None:
                print(match.group(1) + " World: " + match.group(2))
            match=re.search('([0-9\.]+ [0-9:]+).+\[Behaviour\] Initialized PlayerAPI "(.+)" is remote',line)
            if match != None:
                print(match.group(1) + "  User: " + match.group(2))
            line = self.log_file.readline()

    def on_stop(self):
        if self.log_file != None:
            self.log_file.close()

def watch():
    event_handler = MyHandler([LOG_PREFIX+"*"+LOG_EXTENSION])
    observer = Observer()
    observer.schedule(event_handler, LOG_DIRECTORY, recursive=False)
    observer.start()
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        event_handler.on_stop()
        observer.stop()
    observer.join()


if __name__ == "__main__":
    watch()