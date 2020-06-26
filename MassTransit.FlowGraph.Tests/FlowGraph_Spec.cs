using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Assent;
using MassTransit.Initializers;
using MassTransit.Testing;
using Newtonsoft.Json;
using Xunit;

namespace MassTransit.FlowGraph.Tests {
    public class FlowGraph_Spec {
        [Fact]
        public async Task BuildingFlowGraph_ShouldBuild() {
            
            EndpointConvention.Map<EFoo>(new Uri("queue:input-queue"));
            
            var harness = new InMemoryTestHarness() {
                TestTimeout = TimeSpan.FromSeconds(1)
            };

            harness.Consumer(() => new AFooConsumer());
            harness.Consumer(() => new BFooConsumer());
            harness.Consumer(() => new CFooConsumer());
            harness.Consumer(() => new DFooConsumer());
            harness.Consumer(() => new EFooConsumer());
            
            await harness.Start();

            await harness.Bus.Publish<AFoo>(new {
                CorrelationId = Guid.NewGuid()
            });

            await harness.Bus.Publish<BFoo>(new {
                CorrelationId = Guid.NewGuid()
            });

            var graph = await GraphGenerator.Generate(harness);

            var converstationGraphs = string.Join(Environment.NewLine,
                graph.Select(StringGraphRenderer.Render));

            await harness.Stop();

            this.Assent(converstationGraphs);
        }
    }

    public interface ICorrelated {
        public Guid CorrelationId { get; set; }
    }

    public interface AFoo : ICorrelated { }

    public interface BFoo : ICorrelated { }

    public interface CFoo : ICorrelated { }

    public interface DFoo : ICorrelated { }

    public interface EFoo : ICorrelated { }


    public class AFooConsumer : IConsumer<AFoo> {
        public async Task Consume(ConsumeContext<AFoo> context) {
            await context.Publish<BFoo>(new {
                CorrelationId = Guid.NewGuid()
            });

            await context.Publish<BFoo>(new {
                CorrelationId = Guid.NewGuid()
            });
        }
    }

    public class BFooConsumer : IConsumer<BFoo> {
        public async Task Consume(ConsumeContext<BFoo> context) {
            await context.Publish<CFoo>(new {
                CorrelationId = Guid.NewGuid()
            });

            await context.Send<EFoo>(new {
                CorrelationId = Guid.NewGuid()
            });
        }
    }

    public class CFooConsumer : IConsumer<CFoo> {
        public async Task Consume(ConsumeContext<CFoo> context) {
            await context.Publish<DFoo>(new {
                CorrelationId = Guid.NewGuid()
            });

            await context.Publish<DFoo>(new {
                CorrelationId = Guid.NewGuid()
            });
        }
    }

    public class DFooConsumer : IConsumer<DFoo> {
        public Task Consume(ConsumeContext<DFoo> context) {
            return Task.CompletedTask;
        }
    }

    public class EFooConsumer : IConsumer<EFoo> {
        public async Task Consume(ConsumeContext<EFoo> context) {
            await context.Publish<DFoo>(new {
                CorrelationId = Guid.NewGuid()
            });
        }
    }
}