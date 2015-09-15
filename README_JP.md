Merona.cs
====
This japanese documentation is written for a self-studying purpose.<br>

Meronaは難しくて複雑なゲームサーバプログラミングの不便の解消して、サーバロジックの作成する/*FIXME*/ため作ったゲームサーバフレームワークであります。

メロナの特徴
----
* __複雑な知識が必要なし__
* __C#　ベイス__
* __シングルスレド__
* __パチットメタプログラミング__
* __フレキシブル__

チャットサーバ例示
----
簡単なチャットサーバならコードの作成無し、ただパケットスキーマの作成することだけで具現することができます。
```c#
[PgenTarget]
public class MyChattingPackets{
  // Join 패킷을 보낸 클라이언트를 'chat'채널에 가입시킵니다.
  // 그 후 새로운 클라이언트가 입장함을 알리기 위해 현재 패킷을 'chat'채널에 방송합니다.
  [Join("chat")]
  [AutoResponse("chat")]
  public class Join {
    [Forward]
    public String nickname;
  };
  
  // Leave 패킷을 보낸 클라이언트를 'chat'채널에서 탈퇴시킵니다.
  // 위와 같이 퇴장함을 알리기 위해 'chat' 채널에 패킷을 방송합니다.
  [Leave("chat")]
  [AutoResponse("chat")]
  public class Leave {
    [S2C]
    public String nickname;
  }
  
  // 메세지 패킷을 자동으로 'chat'채널에 방송합니다.
  [AutoResponse("chat")]
  public class ChatMessage {
    [Forward]
    public String message;
    
    [S2C]
    public String nickname;
  }
};
```
```c#
void Main(String[] args) {
  var server = new Server(Config.defaults);
  server.Start();
  
  while(true) {
    Console.WriteLine("running...");
    Thread.Sleep(1000);
  }
}
```

ドックリンク
----
[ドックトップページ](doc)<br>
[用語の整理](doc/jinwoo)<br>
[API レファレンス](doc/api)<br>
[ガイド](doc/guide)<br>
[メロナのプロジェクトたちについて](doc/projects)<br>

メロナのプロジェクトたち
----
* [Merona.Pgen](https://github.com/pjc0247/Merona.Pgen.cs)
* [Merona.JsonProtocol](https://github.com/pjc0247/Merona.JsonProtocol.cs)
* [Merona.Http](https://github.com/pjc0247/Merona.Http.cs)<br>
* [Merona.Apns](https://github.com/pjc0247/Merona.Apns.cs)
* [Merona.Migration](https://github.com/pjc0247/Merona.Migration.cs)
