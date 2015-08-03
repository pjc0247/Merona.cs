Merona.cs
====

싱글 스레드 기반 게임 서버 프레임워크

<br><br>

[doc](doc)

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
    디렉션
    ```c#
    class Login : Packet {
        [C2S]
        public string id;
        [C2S]
        public string password;
        
        [S2C]
        public bool result;
    }
    ```
    데이터 오토바인딩
    ```c#
    class QueryMyName : Packet {
      [S2C]
      [Bind("player.name")]
      public string name;  
    }
    ```
    채널
    ```c#
    [Join("world.map")]
    class Join : Packet {
    	[S2C]
    	[Bind("player.id")]
    	string player_id;
    }
    
    [Leave("world.map")]
    class Leave : Packet {
    	[S2C]
    	[Bind("player.id")]
    	string player_id;
    }
    ```
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

* __ORM__
```c#
```
