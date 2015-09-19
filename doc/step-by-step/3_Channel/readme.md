Channel
====

채널은 Session들의 집합입니다. 경로(path)를 가지기 때문에 인스턴스 대신 경로로 표현할 수 있습니다.
<br><br>
Channel.Pool을 이용하여 지정된 경로의 채널에 Session을 가입/탈퇴시키고, 특정 채널에 패킷을 방송할 수 있습니다.

```c#
class JoinRoom : Packet {
  public String name;
}
class LeaveRoom : Packet {
  public String name;
}
class Broadcast : Packet {
  public String name;
  public String message;
}
```
```c#
class TestService : Service {
  [Handler(typeof(JoinRoom))]
  public void OnJoinRoom(Session session, JoinRoom packet) {
    // Channel.Pool을 사용하려면 current 프로퍼티를 이용해 인스턴스를 얻어옵니다.
    // room.ROOM_NAME 채널에 현재 세션을 가입시킵니다.
    Channel.Pool.current.Join(
      $"room.{packet.name}", session);
  }

  [Handler(typeof(LeaveRoom))]
  public void OnLeaveRoom(Session session, LeaveRoom packet) {
    // room.ROOM_NAME 채널에서 탈퇴시킵니다.
    Channel.Pool.current.Leave(
      $"room.{packet.name}", session);
  }

  [Handler(typeof(Broadcast))]
  public void OnBroadcast(Session session, Broadcast packet) {
    // room.ROOM_NAME 채널에 패킷을 방송합니다.
    // 해당 채널에 Join된 모든 클라이언트에게 전송됩니다.
    Channel.Pool.current.Broadcast(
      $"room.{packet.name}", packet);
  }
}
```
채널 경로
----
채널 경로는 상위 채널과 하위 채널의 개념을 가지며, 각 계층은 `.`로 구분됩니다.<br>
오른쪽으로 내려갈 수록 하위 채널을 뜻합니다.
```c#
"area7.chatting"
// 하위 채널을 표기할 때 위의 방법보다 아래의 방법이 권장됩니다.
"area.7.chatting"
```

와일드카드 사용하기
----
패킷 경로(path)에는 와일드카드를 사용할 수 있습니다.<br>
아래의 코드는 방 이름에 관계 없이 생성된 모든 방에 브로드캐스팅합니다.
```c#
Channel.Pool.current.Broadcast(
  "room.*", packet);
```
와일드카드는 여러 번 사용될 수 있습니다.
```c#
아래의 경로는 월드맵 모든 에리어에 존재하는 모든 파티들을 가리킵니다.
"worldmap.area.*.party.*"
```

특정 채널 경로를 구독하는 핸들러 작성하기
----
서비스의 핸들러에서는 패킷 타입 대신 특정 채널로 들어오는 모든 패킷을 핸들링할 수 있습니다.
```c#
// 패킷 타입 대신 구독할 채널 경로를 넣습니다.
// 
[Handler("room.*")]
public void OnRoom(Session session, Packet packet){
}
```

Raw Channel 사용하기
---
아래의 예제는 Channel 인스턴스를 직접 만들어 제어하는 예제입니다.<br>
이 방법은 권장되지 않습니다.
```c#
var channel = new Channel("CHANNEL_PATH");

channel.Join(session);
channel.Leave(session);
channel.Broadcast(packet);
```
