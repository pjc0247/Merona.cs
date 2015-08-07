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
