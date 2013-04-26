UnityでMessagePack使うの調べてみる
===============

UnityでJSONやめてMessagePack使うか検討中

Msgpackのライブラリ
--------------

### perl 

[Data::MessagePack](https://metacpan.org/module/Data::MessagePack)

### unity 

[msgpack-unity](https://github.com/narugami/msgpack-unity)


どんなベンチ?
---------------

1. Unity(C#)で、Dictionaryを作る
2. Unityで(JSON|MsgPack)にシリアライズしてPOSTのbodyにのせてリクエスト
3. server(plack app)でbodyをhashrefにデシリアライズ
4. hashrefを(JSON|MsgPack)にシリアライズしてレスポンスのbodyにのせて返す
5. Unityで受け取ったボディをDictionaryにデシリアライズ

上記のターンを沢山回してパフォーマンスを計測

※UnityPROは持ってないのでFREE盤でShapeUnitのテストとして実行


結果
---------------

    // MsgpackTestCase#JsonBenchmark
    JSON HTTP: n=200
       Time:   306.695
       Rate:   0.6521137/s
    
    // MsgpackTestCase#MsgpackBenchmark
    Msgpack HTTP: n=200
       Time:   287.642
       Rate:   0.6953087/s


MessagePackの方が確かに早いが大差があるとはいいがたい.

どっか別の処理がとても時間かかってそう


捕捉
----------------

plackアプリの単純なベンチとUnityでのMessagePackとJSONの処理もベンチとってみたよ


### perl

[msgpack-perl/bench.pl](msgpack-perl/bench.pl)

    @ count=10

             Rate    json msgpack    none
    json    473/s      --    -10%    -15%
    msgpack 528/s     12%      --     -5%
    none    556/s     18%      5%      --


    @ count=500

              Rate    json msgpack    none
    json    52.4/s      --    -18%    -90%
    msgpack 63.5/s     21%      --    -88%
    none     527/s    905%    729%      --


msgpackの方がだいぶ早いしデータサイズが大きくなればさらに差がつく感じ


### Unity(C#)のデシリアライズ

[msgpack-unity/Assets/Scenes/MsgpackTestCase.cs](msgpack-unity/Assets/Scenes/MsgpackTestCase.cs)

    Json Serialize: n=3000
       Time:   9.993
       Length: 17211
       Rate:   300.2101/s
    
    Msgpack Serialize: n=3000
       Time:   7.701
       Length: 11118
       Rate:   389.5598/s
       
    JSON Deserialize: n=3000
       Time:   19.12
       Length: 17211
       Rate:   156.9038/s
    
    Msgpack Deserialize: n=3000
       Time:   7.83
       Length: 11118
       Rate:   383.1418/s
       
    JSON HTTP: n=200
       Time:   306.695
       Rate:   0.6521137/s
    
    Msgpack HTTP: n=200
       Time:   287.642
       Rate:   0.6953087/s
    
    CreateData: n=100
       Time:   0.044
       Rate:   68181.82/s

MsgpackとJSONだとデシリアライズで圧倒的に差がある。

あと、データサイズも結構差が出る。


感想
-----------

+ JSONをMessagePackに変更するのは比較的簡単そう
+ msgpack-unityのBoxingPackerを少し弄る必要がある
+ Unityでベンチとるのバックグラウンドで動かせないからスゴくめんどくさいから疲れた


動かしかた
----------

### install

    $ cd msgpack-perl 
    $ cpanm --installdeps .


### start server

    $ cd msgpack-perl 
    $ plackup -p 5511 -r echo.psgi


