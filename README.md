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
    ```
    데이터 오토바인딩
    ```c#
    [Channel("#{channel}")]
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
    채널
    ```c#
    [Join("world.map")]
    class Join : Packet {
    	[S2C]
    	[Bind("player.id")]
    	int pid;
    }
    
    [Leave("world.map")]
    class Leave : Packet {
    	[S2C]
    	[Bind("player.id")]
    	int pid;
    }
    ```

* __ORM__
```c#
```
