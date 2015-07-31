데이터 바인딩
====
로직이 필요 없는 단순 대입 역할만이 필요한 필드는 데이터 바인딩을 이용하여 코드 없이 값을 채울 수 있습니다.<br>
(데이터 바인딩은 S2C 패킷에만 유효합니다.)

```c#
class MySession : Session {
  /* .... */
  
  public Player player { get; set; }
};
```
```c#
// 자기 캐릭터의 이름을 물어보고, 가져오는 패킷
[AutoResponse]
class QueryMyNickname : Packet {
  [S2C]
  [Bind("#{player.nickname}")]
  public string nickname;
};
```
위 패킷에서 nickname필드는 단순히 자기 자신의 player객체에서 name의 값만 읽어와서 대입하는 로직만이 필요합니다.<br>
이런 상황에서 데이터 바인딩이 출동하면 굳이 패킷 핸들러를 작성하지 않고도 패킷에 값을 채워서 자동으로 리스폰스 하도록 동작시킬 수 있습니다.

