Lazy
====

Lazy는 패킷 디스크립터가 실제로 호출되는 시점에 적절한 값을 바인딩시킵니다.

```c#
class MySession : Session {
  public String channel {get;set;}
  public String nickname {get;set;}
}

// 전송자 Session의 channel 프로퍼티가 대입됨
[AutoResponse(Lazy = "channel")]
class ChatMessage {
  public String message;
  
  // 전송자 Session의 nickname 프로파티가 대입됨
  [S2C]
  [Bind(Lazy = "nickname")]
  public String nickname;
}
```
