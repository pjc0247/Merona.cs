Forward
====

Forward는 AutoResponse패킷일 때 클라이언트에서 수신받은 Request 패킷의 필드를 그대로 복사하여 다시 전송합니다.

```c#
[AutoResponse]
class EchoMessage {
  // message 필드는 클라이언트에서 보낸것과 동일하게 
  // 채워진 후 AutoResponse를 통해 echo-back 됩니다.
  [Forward]
  public String message;
}
```
