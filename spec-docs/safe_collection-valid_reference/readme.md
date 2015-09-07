SafeContainer & ValidReference
====

Session은 언제라도 클라이언트에 의한 세션 종료에 의해서 Reset될 수 있는 객체이며, 이러한 Session을 컬렉션에 담아 놓거나 변수에 저장해 놓는 행위는 위험할 수 있습니다.

```C#
class MatchMakerService {
  private Queue<Session> pendingSessions {get;set;}
  
  public OnJoinMatch(Session session, JoinMatchPacket packet) {
    pendingSessions.Push(session);
    
    if(pendingSessions.Count >= 2) {
      var p1 = pendingSessions.Pop();
      var p2 = pendingSessions.Pop();
      
      MakeMatch(p1, p2);
    }
  }
}
```

위의 코드에서 첫번째 클라이언트가 JoinMatch 패킷을 전송해 pendingSessions에 추가되었습니다.
하지만 매치 메이킹을 기다리는 도중에 접속을 종료하였고, 그 후 두번째 클라이언트가 JoinMatch 패킷을 통해 매칭 요청을 하였지만, 첫번째 클라이언트가 아직 리스트에 남아있으므로 첫번째 클라이언트의 세션 인스턴스와 매칭이 잡히게 됩니다.<br>
여기서 첫번째 클라이언트는 이미 접속이 종료된 상태이므로 Invalid한 세션과 매칭되거나, Session.Pool에 의해 재사용된 다른 클라이언트와 매칭이 되는 버그가 생깁니다.

<br>
이처럼 Invalidate될 수 있거나, 재사용될 수 있는 인스턴스를 저장해 놓는 행위는 위험할 수 있으며, Merona는 이를 위해 SafeCollection과 ValidReferece 두가지 유틸리티 클래스를 제공합니다.
<br>

SafeCollection
----
SafeCollection은 객체가 Invalidate될 때 자동으로 컬렉션에서 제거시킵니다.
```C#
class MatchMakerService {
  private SafeCollection<Session> pendingSessions {get;set;}
  
  /* ... */
}
```
문제가 있었던 첫번째 예제 코드를 위와 같이 수정하면, 첫번째 클라이언트의 연결이 종료되었을 때 자동으로 컬렉션을 제거되므로 두번째 클라이언트가 JoinMatch를 시도해도 매칭을 시도하는 코드로 더이상 진행되지 않습니다.

<br>
ValidReference
----
ValidReference는 단일 객체에 대해 현재 객체가 유효한 상태인지를 검사합니다.<br>
객체를 생성할 당시의 스냅샷이 저장되므로, 대상 객체가 Invalid상태가 되었다가 다시 Valid한 상태가 되어도 ValidReference는 Invalid상태로 남게 됩니다.
```c#
var vr = new ValidReference(session);
```
