커스텀 패킷 디스크립터
====
```c#
class SomePacket : Packet {
  [Sha256]
  public String verySecretData;
}
```
verySecretData필드는 전송 되기 이전에 자동으로 Sha256암호화됩니다.
<br>
<br>

http://pjc02478.github.io/CustomPacketDescriptor/
