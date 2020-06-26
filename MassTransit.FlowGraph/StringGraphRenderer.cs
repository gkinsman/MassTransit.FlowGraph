using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MassTransit.FlowGraph {
    public static class StringGraphRenderer {
        public static string Render(Node<ConversationMessage> rootNode) {
            var stack = new Stack<Node<ConversationMessage>>();
            stack.Push(rootNode);

            var builder = new StringBuilder();

            while (stack.Any()) {
                var current = stack.Pop();

                var whitespace = new string(' ', (current.Depth -1) * 4);
                builder.AppendLine($"{whitespace} {current.Data.EventType} {current.Data.MessageType}");

                foreach(var node in current.Nodes) {
                    stack.Push(node);
                }
            }

            return builder.ToString();
        }
    }
}