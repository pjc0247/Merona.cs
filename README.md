Merona.cs
====
싱글 스레드 기반 게임 서버 프레임워크<br>
シングルスレド基板のゲームサバフレームワーク。

문서 링크
----
[문서](doc)<br>
[API 레퍼런스](doc/api)<br>
[가이드](doc/guide)<br>

메로나는 복잡하고 난해한 게임 서버 프로그래밍의 불편함을 해소하고, 서버 로직 작성에 충실한 프로그래밍을 할 수 있도록 제작된 서버 프레임워크입니다.<br>

메로나의 특징
----
* __복잡한 지식이 필요 없음__<br>
  메로나는 최대한 서버 프로그래밍에 대한 기반 지식이 없어도 서버 프로그래밍이 가능하게끔 만들어졌습니다. 메로나는 고성능 고복잡성의 서버보다는, 성능은 조금 낮더라도 편리하게 개발할 수 있는 환경을 지향합니다. 프로그래머는 서버의 내부가 어떻게 동작하는지보다, 클라이언트들로부터 들어온 각 패킷들을 어떻게 처리할지에 더 집중할 수 있습니다.<br>
  메로나로 서버를 작성할 때는 아래와 같은 지식들은 필요 없습니다.
  * 데이터베이스
  * 네트워크
  * 멀티스레딩과 동기화
* __C#기반__<br>
  메로나는 C#언어로 제작되었으며, C#의 언어적 특성과 기능들을 최대한 살리면서 제작하도록 노력하였습니다. 
* __싱글 스레드__<br>
  메로나는 싱글 스레드 기반의 서버를 제공합니다. 덕분에 복잡한 동기화 작업 때문에 머리아플 일이 없습니다.


채팅 서버 예제
----
간단한 채팅 서버라면 어떠한 서버 코드 작성 없이 단순히 패킷 정의만을 통해서 구현할 수 있습니다.
```c#
[PgenTarget]
public class MyChattingPackets{
  // Join 패킷을 보낸 클라이언트를 'chat'채널에 가입시킵니다.
  // 그 후 새로운 클라이언트가 입장함을 알리기 위해 현재 패킷을 'chat'채널에 방송합니다.
  [Join("chat")]
  [AutoResponse("chat")]
  public class Join {
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
