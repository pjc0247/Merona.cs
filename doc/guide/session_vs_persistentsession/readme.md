Session vs PersistentSession
====

Session
----
각 클라이언트 연결당 1개 씩 생성되는 Connection 기반 스토리지입니다.<br>
이곳에는 연결이 유효한 시간에만 같이 유효한 값들이 저장됩니다. (TCP Socket, 암호화 키, 일회용 닉네임 등)<br>
세션은 항상 메모리에 저장됩니다.

PersistentSession
----
연결 상태에 관계없이 유지되는 스토리지입니다.<br>
이곳에는 연결이 종료된 후에도 유지되야 하는 데이터들이 저장됩니다. (플레이 데이터, 인벤토리 등)<br>
유지되는 세션은 메모리와 DB 두곳에 존재하며, 설정된 autoCommitType 정책에 따라 적절한 시기에 자동으로 동기화됩니다.


Compare
----
|               | Session     | PersistentSession |
| ------------- | ------------- | --------------- |
| 자동 Open     | O           |  X            |
| 모듈간 데이터 공유됨   |   X | O            |
| Connection 당 가질 수 있는 갯수   |   1 | 0 ~ 맘대로            |
