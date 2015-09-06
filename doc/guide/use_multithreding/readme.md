멀티 쓰레드 환경 서버 구축하기
====
Merona는 기본적으로 싱글 스레드 환경을 제공하지만, 사용자의 필요에 따라 멀티 쓰레드 서버 확장할 수 있습니다.
Merona는 기본적으로 스레드에 안전한 Queue를 이용한 이벤트 분배 방식으로 작업을 처리하므로 단순히 Worker 인스턴스를 여러개 생성하는것으로도 멀티 스레드 서버 구축 설정이 완료됩니다.
<br><br>
하지만 멀티 서버를 위한 설정 이외에도 Merona 내부적, 외부에 공개되는 데이터들에 대한 수동적인 동기화 작업이 반드시 필요합니다.

스레드에 안전한 Merona 내부 데이터
----
* Session.Pool
* Server.Event 에 관련된 모든 로직

스레드에 안전하지 않은 Merona 내부 데이터
----
* Channel.Pool
* Channel.TreeDictionary
* CircularBuffer
* DataBinder
