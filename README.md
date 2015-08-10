Merona.cs
====

싱글 스레드 기반 게임 서버 프레임워크<br>
シングルスレド基板のゲームサバフレームワーク。

<br><br>

[doc](doc)


채팅 서버 예제
```c#
[PgenTarget]
public class MyChattingPackets{
  
  [Join("chat")]
  [AutoResponse("chat")]
  public class Join {
    public String nickname;
  };
  
  [Leave("chat")]
  [AutoResponse("chat")]
  public class Leave {
    [S2C]
    public String nickname;
  }
  
  [AutoResponse("chat")]
  public class ChatMessage {
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

packet pipelining
----
__IN__ : Session -> IO -> Unmarshaling -> PreProcess -> Service -> Router<br>
__OUT__ : Session -> PostProcess -> Marshaling -> IO
