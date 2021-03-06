용어 정리
====

Server
----
Merona에서의 서버는 하나의 서버 어플리케이션을 통칭하는 것이 아니라, 하나의 프로세스 내에서 독립 가능한 하나의 서버 인스턴스를 의미합니다.
한 프로세스는 여러개의 서버를 가질 수 있으며, 각 서버들은 독립된 Worker와 세션 풀을 가집니다.
<br>
Meronaでサバは一つのサバアプリを意味するものではなく、一つのプロセス内に独立可能なサバインスタンスを意味します。
一つのプロセスは複数のサバインスタンスを持ってことができます。各サバたちは独立なWorkerとセッションプールを持っています。

Packet
----
Packet이란 서버와 클라이언트가 주고받는 데이터의 기본 단위입니다.
Merona에서의 패킷은 네트워크 프로토콜과 독립적이며, 단순한 Binary 통신 혹은 HTTP 통신을 사용하던 똑같은 패킷 스키마를 사용하는 것이 가능합니다.
<br>
Packetはサバとクライアントが通信するデータの基本単位です。MeronaでPacketはネーコワクプロトコルと独立的にて、ただのBinaryプロトコルまだはHTTPプロトコルを使っても同じPacketスキーマを使うことができます。

Model
----
Model이란 연관성 있는 데이터들의 집합(Player -> level, name, class등...)입니다. 이 데이터들은 Merona가 데이터베이스와 통신하여 관리하는 기본 단위임과 동시에 패킷에 포함될 수도 있습니다.
<br>
Modelは連関性あるデータたちの集合（Player -> level, name, class ...)です。このデータたちはMeronaがデータベースと通信して管理する基本単位として、Packetにも追加されることもできます。

Session
----

PersistentSession
----

Safe-Thread
----
안전한 쓰레드란 서버의 Worker쓰레드를 의미합니다.<br>
이 쓰레드는 서버의 모든 자원(세션, 채널, 모델...)에 접근할 수 있는 유일한 쓰레드입니다.
<br>
安全なスレドはサバのWorkerスレドを意味します。<br>
このスレドにはサバの全てのリソースにアクセスすることができる唯一のスレッドです。

Non-Safe-Thread
----
안전하지 않은 쓰레드란, 안전한 쓰레드를 제외한 모든 쓰레드를 의미합니다.<br>
.Net의 쓰레드풀 쓰레드, 프로세스의 메인 쓰레드, 직접 생성한 쓰레드등은 모두 안전하지 않은 쓰레드이며, 해당 쓰레드에서 서버 자원에 접근할 경우 비정상적인 동작이 일어날 수 있습니다.
