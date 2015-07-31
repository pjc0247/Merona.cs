채널
====

```c#
[Join("world.map")]
class JoinMap : Packet {
  [S2C]
  [Bind("#{player.id})]
  public string player_id;
};
[Leave("world.map")]
class LeaveMap : Packet {
  [S2C]
  [Bind("#{player.id})]
  public string player_id;
};
```
