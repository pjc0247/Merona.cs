오토 리스폰스
====
만약 패킷의 모든 S2C필드가 데이터 바인딩에 의해 값이 채워졌고 추가적인 로직이 필요하지 않다면 __AutoResponse__디스크립터를 지정하여 해당 패킷이 추가적인 핸들러 없이도 자동으로 리스폰스 패킷을 보내도록 지정할 수 있습니다.

```c#
[AutoResponse]
class HelloPacket : Packet {
  [S2C]
  [Binding("hello_auto_response")]
  public string text;
};
```
위 패킷은 __AutoResponse__가 지정되었으므로 자동으로 HelloPacket에 대한 리스폰스를 보내고, 데이터 바인딩에 의해 text 필드에는 "hello_auto_response"값이 채워지게 됩니다.
