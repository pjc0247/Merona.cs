ProtocolBased Hook
====

프로토콜을 정의하고 해당 프로토콜에 일치하는 모든 패킷에 대해 처리를 수행할 수 있습니다.
<br>
프로토콜에 정의된 필드를 모두 가지고 있느냐가 발동 조건이며, 해당 조건을 만족하면 따로 상속 등을 하지 않아도 됩니다.
(이는 go의 interface와 유사합니다.)

```c#
class LoginProtocol : IProtocol {
  public String id;
  public String password;

  internal protected override void OnPostProcess(LoginProtocol packet) {
    packet.password = SomeSha256Func(packet.password);
  }
}
```
위의 LoginProtocol은 String 타입의 `id`, `password` 필드를 가진 모든 패킷에 대해서 자동으로 `Sha256` 해싱을 수행합니다.

```c#
class PositionProtocol : IProtocol {
  public float x;
  public float y;

  internal protected override void OnPreProcess(PositionProtocol packet) {
    /* convert ClientWorld -> ServerWorld location */
  }
  internal protected override void OnPostProcess(PositionProtocol packet) {
    /* convert ServerWorld -> ClientWorld location */
  }
}
```
위의 PositionProtocol은 `x`, `y` 필드를 가진 모든 패킷에 대해 서버좌표계 <-> 클라이언트 좌표계간의 상호 변환을 수행합니다.
