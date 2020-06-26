using System.Collections.Generic;

namespace MassTransit.FlowGraph {
    public class Node<T>
    {
        public Node(T data, int depth)
        {
            Data = data;
            Depth = depth;
            Nodes = new List<Node<T>>();
        }

        public T Data { get; }
        public int Depth { get; }
        public IList<Node<T>> Nodes { get; }
    }
}