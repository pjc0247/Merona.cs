Merona.cs
====

features
----
* __패킷 디스크립터__
```c#
class Player : Model {
  public string name;
  public int speed;
  public int x, y;
}
class Move : Packet {
  public int direction;
  
  [S2C]
  [Bind("player.id")]
  public string player_id;
  [S2C]
  [Bind("player.speed")]
  public int speed;
}
```

* __ORM__
```c#
```
