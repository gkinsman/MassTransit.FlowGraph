using System;

namespace MassTransit.FlowGraph {
    public class ConversationMessage {
        public ConversationMessage(EventType type,
            Guid? conversationId,
            Type messageType,
            Guid? initiatedBy,
            Guid? correlationId) {
            EventType = type;
            MessageType = messageType;
            InitiatedBy = initiatedBy;
            CorrelationId = correlationId;
            ConversationId = conversationId;
        }

        public Guid? ConversationId { get; }
        public EventType EventType { get; }
        public Type MessageType { get; }
        public Guid? InitiatedBy { get; }
        public Guid? CorrelationId { get; }
    }
}