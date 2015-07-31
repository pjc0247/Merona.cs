모델과 관계 설정
====

```c#
class Player : Model {
  public string id;
  public string password;
  
  public string nickname;
};
```
```c#
class Login : Packet {
  [C2S]
  [MemberOf(typeof(Player))]
  public string id;
  [C2S]
  [MemberOf(typeof(Player))]
  public string password;
  
  [S2C]
  [MemberOf(typeof(Player))]
  public string nickname;
  [S2C]
  public bool result;
};
```
