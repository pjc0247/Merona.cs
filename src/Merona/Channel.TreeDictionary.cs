using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public sealed partial class Channel
    {
        /* KEY   : Channel.Path
         * VALUE : Channel */
        internal class TreeDictionary
        {
            private Node root { get; set; }

            private class Node
            {
                private Dictionary<int, Node> children { get; set; }
                private Dictionary<int, Channel> items { get; set; }
                public int depth { get; private set; }

                public Node(int depth)
                {
                    this.children = new Dictionary<int, Node>();
                    this.items = new Dictionary<int, Channel>();
                    this.depth = depth;
                }

                public Channel Add(Channel.Path path)
                {
                    int key = path.raw[depth];

                    if (path.raw.Length - 1 == depth)
                    {
                        return items[key] = new Channel(path);
                    }
                    else
                    {
                        if (!children.ContainsKey(key))
                            children[key] = new Node(depth + 1);

                        return children[key].Add(path);
                    }
                }
                public void Remove(Channel.Path path)
                {
                    int key = path.raw[depth];

                    if (path.raw.Length - 1 == depth)
                    {
                        items.Remove(key);
                    }
                    else if (children.ContainsKey(key))
                    {
                        children[key].Remove(path);
                    }
                    else
                    {
                        /* 경로 없음 */
                    }
                }

                public void FindOne(Channel.Path path,ref List<Channel> results)
                {
                    int key = path.raw[depth];

                    if (path.raw.Length - 1 == depth)
                    {
                        if (items.ContainsKey(key))
                        {
                            results.Add(items[key]);
                            return;
                        }
                    }
                    else if (children.ContainsKey(key))
                        children[key].FindOne(path, ref results);
                }
                public void Find(Channel.Path path, ref List<Channel> results)
                {
                    int key = path.raw[depth];

                    if (path.raw.Length - 1 == depth)
                    {
                        if (items.ContainsKey(key))
                            results.Add(items[key]);
                        else if (key == Channel.Path.asterisk)
                        {
                            foreach (var item in items)
                                results.Add(item.Value);
                        }
                    }
                    else if (key == Channel.Path.asterisk)
                    {
                        foreach (var child in children)
                        {
                            child.Value.Find(path, ref results);
                        }
                    }
                    else if (children.ContainsKey(key))
                    {
                        children[key].Find(path, ref results);
                    }
                }
            }

            public TreeDictionary()
            {
                root = new Node(0);
            }

            public Channel Add(Channel.Path path)
            {
                return root.Add(path);
            }
            public void Remove(Channel.Path path)
            {
                root.Remove(path);
            }

            /// <summary>
            /// 지정된 경로에 일치하는 모든 채널들을 검색한다.
            /// </summary>
            /// <param name="path">탐색할 경로 (와일드카드 사용 가능)</param>
            /// <returns>일치하는 채널들</returns>
            public List<Channel> Query(Channel.Path path)
            {
                var results = new List<Channel>();

                if (path.isFixed)
                    root.FindOne(path, ref results);
                else
                    root.Find(path, ref results);

                return results;
            }
        }
    }
}
