using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.Testing;

namespace MassTransit.FlowGraph {
    public static class GraphGenerator {
        public static async Task<IList<Node<ConversationMessage>>> Generate(
            BusTestHarness harness) {
            var messages = new List<ConversationMessage>();

            var published = await harness.Published.SelectAsync<object>().ToListAsync();
            var sent = await harness.Sent.SelectAsync<object>().ToListAsync();

            messages.AddRange(published.Select(r =>
                new ConversationMessage(EventType.Publish, r.Context.ConversationId, r.MessageType,
                    r.Context.InitiatorId, r.Context.CorrelationId)));
            messages.AddRange(sent.Select(r =>
                new ConversationMessage(EventType.Send, r.Context.ConversationId,
                    r.MessageType, r.Context.InitiatorId, r.Context.CorrelationId)));

            var conversations = messages.GroupBy(m => m.ConversationId).ToList();

            return conversations
                .Select(c => GenerateConversationGraph(c.ToList())).ToList();
        }

        static Node<ConversationMessage> GenerateConversationGraph(IList<ConversationMessage> messages) {
            var initiator = messages.Single(m => m.InitiatedBy == null);

            var rootNode = new Node<ConversationMessage>(initiator, 1);

            var stack = new Stack<Node<ConversationMessage>>();

            stack.Push(rootNode);

            while (stack.Any()) {
                var element = stack.Pop();
                var nextMessages = messages
                    .Where(m => m.InitiatedBy == element.Data.CorrelationId);

                foreach(var nextMessage in nextMessages) {
                    var newNode = new Node<ConversationMessage>(nextMessage, element.Depth + 1);
                    element.Nodes.Add(newNode);
                    stack.Push(newNode);
                }
            }

            return rootNode;
        }
    }
}