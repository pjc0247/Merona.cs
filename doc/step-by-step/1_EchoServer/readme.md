Echo Server 작성하기
====

Merona로 제작되는 서버는 크게 3가지 구성 요소로 이루어집니다.<br>
첫번째는 주고받는 데이터의 단위인 __Packet__<br>
두번째는 패킷을 처리하는 단위인 __Service__<br>
세번째는 서버 인스턴스 그 자체인 __Server__입니다<br>
이번 장에서는 간단한 EchoServer를 작성하며 Merona의 세가지 구성 요소들을 간단하게 살펴 봅니다.

Packet
----
Packet은 Peer와 Peer가 주고받는 데이터의 기본 단위입니다.<br>
패킷의 모든 데이터는 프로퍼티가 아닌 필드로 작성되어야 합니다.
```c#
using Merona;

// 모든 패킷은 Packet 클래스를 상속 받습니다.
public EchoPacket : Packet {
  public String message;  
}
```

Service
----
서비스는 패킷을 처리하는 단위입니다.<br>
하나의 서비스는 연관성 있는 패킷을을 처리하도록 분리하는것이 좋습니다.<br>
(예: 로그인을 처리하는 AuthService, 매치 메이킹을 처리하는 MatchService, 채팅을 처리하는 ChatService ...)
```c#
using Merona;

// 모든 서비스는 Service 클래스를 상속받습니다.
public EchoService : Service {
  // Handler 어트리뷰트를 사용하여 이 메소드가 특정 타입의 패킷을
  // 처리함을 나타냅니다.
  [Handler(typeof(EchoPacket)]
  public EchoPacket OnEchoPacket(Session session, EchoPacket packet) {
    // Handler 메소드에서 패킷을 리턴하는 행동은,
    // session.Send와 동일한 효과를 가집니다.
    return packet;
  }
}
```

Server
----
서버는 말 그대로 서버입니다. 여러개의 서비스들이 모여 만들어지며,<br>
모든 Io작업, 서비스들에게 알맞은 패킷 분배, 로거, DB 연결등을 가집니다.<br>
하나의 프로세스(exe)는 여러개의 서버를 가질 수 있습니다.
```c#
using System.Threading;
using Merona;

public Program {
  public static void Main(String[] args) {
    var server = new Server();
    
    server.AttachService<EchoService>();
    server.Start();
    
    // Start 이후에 프로그램이 블록되지 않으므로
    // 수동으로 Thread.Sleep 또는 Console.Read를 사용하여
    // 대기시켜야 합니다.
    while(true) {
      Console.WriteLine("Running....");
      Thread.Sleep(1000);
    }
  }
}
```
