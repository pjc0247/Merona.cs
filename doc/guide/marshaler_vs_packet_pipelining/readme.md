마샬러와 패킷 파이프라이닝의 비교
====

|               | Marshaler     | Packet Pipeline | Packet Descriptor |
| ------------- | ------------- | --------------- | ----------------- |
| 실행 스레드   | Non-Safe      | Safe            | Safe              |
| 처리 데이터   | byte[]        | Packet          | Packet's Single Field) |
| 세션 접근 가능| X        | O          | O                           |
| 목적          | 네트워크 프로토콜과 패킷 간의 의존성 분리  | 패킷의 각 필드들에 대한 가공 | 패킷의 단일 필드에 대한 가공 |
