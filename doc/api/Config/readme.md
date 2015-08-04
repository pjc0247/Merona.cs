Config
====
Config는 서버 인스턴스에 대한 설정값들의 집합입니다.

name
----
서버의 이름을 지정하거나 가져옵니다.

host
----
서버의 호스트 주소를 지정하거나 가져옵니다.

port
----
서버의 포트를 지정하거나 가져옵니다.

schedulerMaxInterval
----
서버가 깨어나는 주기를 설정하거나 가져옵니다.<br>
서버에 아무런 이벤트가 쌓이지 않아도 설정된 시간이 경과하면 깨어나서 다음 루프로 넘어갑니다.

sessionType
----
각 클라이언트에 바인당할 Session 클래스의 타입을 가져오거나 설정합니다.<br>
해당 클래스는 Session으로부터 상속받은 클래스여아 합니다.

sessionRingBufferSize
----

sessionRecvBufferSize
----

sessionPoolSize
----

dbHostName
----

dbDatabaseName
----

endian
----
패킷의 바이트 오더를 설정하거나 가져옵니다.
