PersistentSession
====
PersistentSession은 지속적으로 유지되는 세션으로 수동으로 삭제하지 않는 한 서버를 재시작하거나 연결이 끊겨도 지워지지 않습니다. 이 값은 유저가 정의한 유니크한 키(Key)에 의해 식별되기 때문에, 사용하기 전 수동으로 키를 지정하여 생성하거나 로드하는 작업이 선행되어야 합니다.

isOpened
----
현재 PersistentSession이 사용 가능한 상태인지 조사합니다.

autoCommitMode
----
데이터를 자동으로 커밋하는 모드를 설정합니다.
* __AutoCommitMode__
  * AfterSessionClosed<br>
    세션이 정리된 후 자동으로 커밋합니다.
  * AfterRequest<br>
    요쳥에 대한 처리가 끝난 후 자동으로 커밋합니다.
  * None<br>
    자동으로 커밋하지 않습니다. 직접 Commit 메소드를 호출하여 커밋하지 않으면 변경된 내용은 유실됩니다.

CreateAsync(key)
----
유니크 키(Key)를 기반으로 새 PersistentSession을 생성합니다.<br>
만약 이전에 동일한 키를 가진 항목이 존재할 경우 익셉션이 발생합니다.

OpenAsync(key)
----
유니크 키(Key)를 기반으로 PersistentSession을 로드합니다.<br>
만약 이전에 동일한 키로 생성된 PersistentSession이 없을 경우 익셉션이 발생합니다.

CommitAsync
----
현재 PersistentSession의 변경 사항들을 저장합니다.

RemoveAsync
----
현재 오픈된 PersistentSession을 삭제합니다.<br>
단순히 데이터를 비우는것이 아니라 공간을 삭제함으로써 생성할 때 유니크 키(Key)는 다시 사용 가능 상태가 됩니다.


```c#
[Collection("PlayData")]
class PlayData : PersistentSession {
 public long highScore { get; set; }
};
```
