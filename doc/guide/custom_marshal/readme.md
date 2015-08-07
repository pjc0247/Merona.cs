커스텀 마샬러
====

Merona에서 Packet은 네트워크 프로토콜(BinaryStream, Json, HTTP, Websocket...)에 구애받지 않는 단순히 서버와 클라이언트간에 통신하는 데이터 집합의 한 단위입니다.<br>
Merona에서의 Marshal은 패킷이 네트워크 프로토콜과의 독립성을 가질 수 있도록, 패킷이 네트워크상에서 어떤 형태로 보내져야 하고, 어떠현 형태로 받아지는지의 상호 변환 방법을 의미합니다. (IO <-> Marshal <-> Packet)
<br>
