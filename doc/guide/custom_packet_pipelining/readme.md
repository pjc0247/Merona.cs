패킷 파이프라이닝
====
패킷 파이프라이닝은 클라이언트로부터 수신된 패킷이 각 서비스와 라우터들에게 배달되기 전, 또는 세션에 패킷 전송이 요청된 후에 발송되기 전에 처리 로직을 끼워 넣어 패킷을 가공 할 수 있도록 해줍니다.<br>
PreProcessor와 PostProcessor는 서버 인스턴스에 고유하며, 안전한 스레드에서 실행되기 때문에 세션 객체에 접근할 수 있습니다.
```c#
server.AddPreProcessor(delegate (Session session, Packet packet){
  Console.WriteLine("OnPacketPreProcessor");
}, 0);
```
```c#
server.AddPostProcessor(delegate (Session session, Packet packet){
  Console.WriteLine("OnPacketPostProcessor");
}, 0);
```
