디렉션
====
동일한 기능에 대해 Request(Clinet-to-Server)/Response(Server-to-Client) 두가지 패킷으로 분리된 구조에 __디렉션__디스크립터를 사용하면 하나의 통합적인 클래스로 묶어서 관리할 수 있습니다.
```c#
class LoginRequest : Packet {
  public string id;
  public string password;
};
class LoginResponse : Packet {
  public bool result;
};
```
위의 LoginRequest/Response 클래스는 아래 코드의 통합된 Login 패킷으로 대체할 수 있습니다.
```c#
class Login : Packet {
  [C2S]
  public string id;
  [C2S]
  public string password;
  
  [S2C]
  public bool result;
};
```

만약 필드에 디렉션이 지정되지 않았다면, 해당 필드는 S2C/C2S 양쪽 에 다 해당하는 공통 필드로 인식됩니다.

```c#
class ChatMessage : Packet {
  public string message; // 채팅 패킷에서 메세지는 클라에서 보낼 때, 서버에서 방송할 때의 경우 둘 다 필요하다.
  
  [S2C]
  public string player_id; // 발신자의 아이디, 서버에서 방송할 때만 필요하다.
};
```
