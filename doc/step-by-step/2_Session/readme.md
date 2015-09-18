Session
====

세션은 서버가 클라이언트에 대해 가지는 임시 저장소입니다.<br>
기본적으로, 세션에 저장된 값들은 연결 시간 동안만 유효하며, 연결이 종료되면 모두 날아갑니다.

```c#
// 세션을 상속하여 구현하면 추가 커스텀 데이터를 저장하고
// OnConnect, OnDisconnect등의 이벤트를 통지받을 수 있습니다.
public class MySession : Session {
  // 세션에 닉네임을 저장합니다.
  public String nickname {get;set;}

  protected override void OnConnect() {
    // 새로운 클라이언트가 연결되어, 현재 세션이 바인딩 되었을 때 호출됩니다.
  }
  protected override void OnDisconnect() {
    // 클라이언트와의 연결이 해지되어, 현재 세션에서 언바인딩 되었을 때 호출됩니다.
  }
}
```

이렇게 상속한 커스텀 세션은 서버의 설정을 변경해야 정상적으로 반영됩니다.
```c#
var config = Config.defaults;
config.sessionType = typeof(MySession);
```

서비스의 핸들러에서 Session 대신 커스텀 세션을 직접 넘겨 받을 수 있습니다.
```c#
class TestService : Service {
  [Handler(typeof(TestPacket))]
  public void OnTestPacket(MySession session, TestPacket packet) {
    /* ... */
  }
}
```
