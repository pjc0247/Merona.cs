Merona.cs
====

싱글 스레드 기반 게임 서버 프레임워크

<br><br>

[doc](doc)


packet pipelining
----
__IN__ : Session -> IO -> Unmarshaling -> PreProcess -> Service -> Router<br>
__OUT__ : Session -> PostProcess -> Marshaling -> IO
