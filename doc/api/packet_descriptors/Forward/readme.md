Forward
====

Target
----
__Field__

PostProcess
----
수신한 패킷의 해당 필드의 값을 그대로 송신 패킷의 해당 필드에 복사합니다.<br>
이 디스크립터는 패킷이 AutoResponse일 때 유용합니다.

```c#
public class Join : Packet {
  // C2S : 클라이언트에서 설정한 닉네임을 서버에 알리기 위해 사용
  // S2C : 다른 플레이어들에게 새로 접속한 플레이어의 닉네임을 알리기 위해 사용
  // Forward : 클라이언트에서 수신된 값이 그대로 S2C 패킷으로 복사됩니다.
  [Forward]
  public String nickname;
}
```
