RPC
====

기존의 Merona의 방식대로 서버에서 클라이언트에 이벤트(게임 시작, 유닛 스폰 등)를 전송하려면 이벤트를 전송하기 위한 패킷을 설계하고, 보내고, 클라이언트에서는 해당 패킷을 수신받아 핸들링하는 절차가 필요했습니다.
<br>
RPC는 Merona.Go에 포함되는 직접적인 메소드 콜을 통한 이벤트 전달 방식으로, 서버에서 클라이언트의 메소드를 직접 호출하는 방식의 인터페이스를 제공합니다.

* __Client__
```c#
public class Room {
  [RPC]
  public void OnGameStart() {
    /* change scene -> game 등등... */
  }
}
```

* __Server__
```c#
public class Room : ServerObject {
  public void OnGameStart() {
    RPC.FromRemote();
  }
  
  public void OnJoin(Player player) {
    players.Add(player);
    
    if(players.Count == 4)
      OnGameStart();
  }
}
```
