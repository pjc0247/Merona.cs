커스텀 마샬러
====

Merona에서 Packet은 네트워크 프로토콜(BinaryStream, Json, HTTP, Websocket...)에 구애받지 않는 단순히 서버와 클라이언트간에 통신하는 데이터 집합의 한 단위입니다.<br>
Merona에서의 Marshal은 패킷이 네트워크 프로토콜과의 독립성을 가질 수 있도록, 패킷이 네트워크상에서 어떤 형태로 보내져야 하고, 어떠현 형태로 받아지는지의 상호 변환 방법을 의미합니다. (IO <-> Marshal <-> Packet)
<br>
기본적으로는 C 프로그램의 struct와 호환 가능한 형태를 가지는 Binary 형태의 마샬러를 내장하고 있으며, 이외의 특수한 프로토콜이나, 암호화를 구현하려면 __IMarshalContext__ 클래스를 상속받아 커스텀 마샬러를 구현해야 합니다. 커스텀 마샬러는 스트리밍 파싱, 상태 머신을 지원하기 위해 각 세션에 고유한 인스턴스를 가지며, 세션이 Reset될 때 같이 새로 생성됩니다.
<br>
마샬러는 안전하지 않는 스레드에서 실행되기 때문에, 단순히 Serialize/Deserialize작업 이외에 서버 또는 세션에 관련된 작업을 수행할 수 없습니다.

```c#
class MyMarshaler : IMarshalContext {
  // 네트워크로부터 수신된 byte 배열로부터 패킷을 생성합니다.
  protected override Packet Deserialize(CircularBuffer<byte> buffer){
    
  }
  
  // 네트워크에 보낼 패킷들의 배열로부터 바이트 배열을 생성합니다.  
  protected override byte[] Serialize(CircularBuffer<Packet> buffer){
    
  }
}
```
