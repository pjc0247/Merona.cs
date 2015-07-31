채널
====

```c#
// 이 패킷을 보낸 클라이언트를 world.map 채널에 가입시킨다.
[Join("world.map")]
class JoinMap : Packet {
  [S2C]
  [Bind("#{player.id}")]
  public string player_id;
};

// 이 패킷을 보낸 클라이언트를 world.map 채널에서 탈퇴시킨다.
[Leave("world.map")]
class LeaveMap : Packet {
  [S2C]
  [Bind("#{player.id}")]
  public string player_id;
};
```
```c#
[Channel("world.map")]
[AutoResponse]
class Move : Packet {
  int direction;

  [S2C]
  [Bind("#{player.id")]
  public string player_id;
  [S2C]
  [Bind("#{player.speed")]
  public string speed;
};
```
