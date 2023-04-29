# WGS Exporter
Most Game Pass for PC / Microsoft Store versions of games use Microsoft GDK / Xbox Live Services to save configuration and user data, which takes care of persisting and syncing it across devices.

One issue with this is if you want to move your saves to another version of the game, it's very hard to do (native game data is obfuscated and stored with additional Xbox data).

This project is focused on reverse-engineering the metadata format, and is aimed to make the export process more acessible.

# Caveats
[Game save API](https://learn.microsoft.com/en-us/gaming/gdk/_content/gc/system/overviews/game-save/game-saves) provided by GDK is a bit limited compared to traditional free-form file-based save management implemented by most games.

There is a concept of hierarchy of Storage space per game per user, which has Storage containers, which in turn contain named blobs of data.

Because of this, it's impossible to do automated 1:1 copy from Microsoft Store version of the game to regular PC version of the game that you might have on Steam or some other service, but generally speaking it's easy to figure out how to move and/or rename the files to make it work for each game.
