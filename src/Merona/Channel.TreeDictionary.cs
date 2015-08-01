using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public sealed partial class Channel
    {
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

                public void Add(Channel channel)
                {
                    int key = channel.raw[depth];

                    if (channel.raw.Length - 1 == depth)
                    {
                        items[key] = channel;
                    }
                    else
                    {
                        if (!children.ContainsKey(key))
                            children[key] = new Node(depth + 1);

                        children[key].Add(channel);
                    }
                }
                public void Remove(Channel channel)
                {
                    int key = channel.raw[depth];

                    if (channel.raw.Length - 1 == depth)
                    {
                        items.Remove(key);
                    }
                    else if (children.ContainsKey(key))
                    {
                        children[key].Remove(channel);
                    }
                    else
                    {
                        /* 경로 없음 */
                    }
                }

                public void Find(Channel channel, ref List<Channel> results)
                {
                    int key = channel.raw[depth];

                    if (channel.raw.Length - 1 == depth)
                    {
                        if (items.ContainsKey(key))
                            results.Add(items[key]);
                        else if (key == Channel.asterisk)
                        {
                            foreach (var item in items)
                                results.Add(item.Value);
                        }
                    }
                    else if (key == Channel.asterisk)
                    {
                        foreach (var child in children)
                        {
                            child.Value.Find(channel, ref results);
                        }
                    }
                    else if (children.ContainsKey(key))
                    {
                        children[key].Find(channel, ref results);
                    }
                }
            }

            public TreeDictionary()
            {
                root = new Node(0);
            }

            public void Add(Channel channel)
            {
                root.Add(channel);
            }
            public void Remove(Channel channel)
            {
                root.Remove(channel);
            }
            public List<Channel> Query(Channel channel)
            {
                var results = new List<Channel>();

                root.Find(channel, ref results);

                return results;
            }
        }
    }
}
