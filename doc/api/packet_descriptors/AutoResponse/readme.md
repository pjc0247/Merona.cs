AutoResponse
====

Target
----
__Packet__

PostProcess
----
패킷을 지정한 채널에 방송하거나 해당 패킷을 전송한 클라이언트에게 자동으로 답장을 보냅니다.

```c#
// 인자 없이 사용할 경우 해당 패킷을 전송한 클라이언트에게만 송신합니다.
[AutoResponse]
class MyPacket : Packet {
  
  [S2C]
  [Bind("Hello World")]
  public Stirng msg;
}
```

```c#
// 인자로 채널 경로를 전달하여 해당 채널에 속한 클라이언트들에게 방송할 수 있습니다.
[AutoResponse("world")]
class MyPacket : Packet {
  
  [S2C]
  [Bind("Hello World")]
  public Stirng msg;
}
```
