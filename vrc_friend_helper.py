#!/usr/bin/env python
from __future__ import print_function

import sys
import time
import subprocess
from watchdog.observers import Observer
from watchdog.events import PatternMatchingEventHandler

# [User]の部分をユーザー名に書き換えてください
# LOG_DIRECTORY = "C:\Users\[User]\AppData\LocalLow\VRChat\VRChat\" # WindowsのPythonで実行する場合
LOG_DIRECTORY = "/mnt/c/Users/wararyo/AppData/LocalLow/VRChat/VRChat" # WSL2のPythonで実行する場合
LOG_PREFIX = "output_log_"
LOG_EXTENSION = "txt"

class MyHandler(PatternMatchingEventHandler):
    def __init__(self, patterns):
        super(MyHandler, self).__init__(patterns=patterns)

    def on_moved(self, event):
        print("on_moved")

    def on_created(self, event):
        print("on_created")

    def on_deleted(self, event):
        print("on_deleted")

    def on_modified(self, event):
        print("on_modified")


def watch():
    event_handler = MyHandler(["*"])
    observer = Observer()
    observer.schedule(event_handler, LOG_DIRECTORY, recursive=True)
    observer.start()
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        observer.stop()
    observer.join()


if __name__ == "__main__":
    watch()