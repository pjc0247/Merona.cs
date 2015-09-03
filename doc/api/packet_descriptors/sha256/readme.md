Sha256
====

PostProcess
----
해당 필드를 Sha256알고리즘으로 해싱합니다. 필드는 반드시 String 타입이어야 합니다.

```c#
class MyPacket : Packet {
  // 이 필드는 전송되기 이전에 자동으로 Sha256 해싱됩니다.
  [Sha256]
  public String password;
}
```
