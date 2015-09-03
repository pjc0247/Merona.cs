Channel.Path
====
채널을 가리키는 경로입니다. <br>
경로에는 와일드 카드 문자(*, **)가 포함될 수 있습니다.

```c#
var path1 = new Channel.Path("hello.world.map");
var path2 = new Channel.Path("hello.world.*");

path1.IsMatch(path2); // true
```

isFixed
----
경로가 와일드 카드 문자를 포함하고 있지 않은 단일 경로인지, 와일드 카드 문자를 포함하여 복수개의 채널을 가리키는 경로인지의 여부를 가져옵니다.

IsMatch (Channel.Path path)
----
경로와 다른 경로가 일치하는지 검사합니다. 이 동작은 와일드 카드 검색을 포함합니다.
