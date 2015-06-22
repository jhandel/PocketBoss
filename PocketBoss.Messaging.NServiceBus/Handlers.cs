using NServiceBus;
using PocketBoss.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Messaging.NServiceBus
{
    public class GetSingleWorkflowTemplateRequestHandler : IHandleMessages<PocketBoss.Messages.Commands.GetSingleWorkflowTemplateRequest>
    {
        private IBus _bus { get; set; }
        public GetSingleWorkflowTemplateRequestHandler(IBus bus)
        {
            _bus = bus;
        }
        public void Handle(Messages.Commands.GetSingleWorkflowTemplateRequest message)
        {
            var processor = new ProcessWorkflowRequests(new NServiceBusService(_bus));
            processor.GetSingleWorkflowTemplateRequestHandler(message);
        }
    }

    public class GetWorkflowInstanceDetailsRequestHandler : IHandleMessages<PocketBoss.Messages.Commands.GetWorkflowInstanceDetailsRequest>
    {
        private IBus _bus { get; set; }
        public GetWorkflowInstanceDetailsRequestHandler(IBus bus)
        {
            _bus = bus;
        }
        public void Handle(Messages.Commands.GetWorkflowInstanceDetailsRequest message)
        {
            var processor = new ProcessWorkflowRequests(new NServiceBusService(_bus));
            processor.GetWorkflowInstanceDetailsRequestHandler(message);
        }
    }

    public class GetWorkflowTemplatesRequestHandler : IHandleMessages<PocketBoss.Messages.Commands.GetWorkflowTemplatesRequest>
    {
        private IBus _bus { get; set; }
        public GetWorkflowTemplatesRequestHandler(IBus bus)
        {
            _bus = bus;
        }
        public void Handle(Messages.Commands.GetWorkflowTemplatesRequest message)
        {
            var processor = new ProcessWorkflowRequests(new NServiceBusService(_bus));
            processor.GetWorkflowTemplatesRequestHandler(message);
        }
    }

    public class InitiateWorkflowRequestHandler : IHandleMessages<PocketBoss.Messages.Commands.InitiateWorkflowRequest>
    {
        private IBus _bus { get; set; }
        public InitiateWorkflowRequestHandler(IBus bus)
        {
            _bus = bus;
        }
        public void Handle(Messages.Commands.InitiateWorkflowRequest message)
        {
            var processor = new ProcessWorkflowRequests(new NServiceBusService(_bus));
            processor.InitiateWorkflowRequestHandler(message);
        }
    }

    public class RecordStateActionHandler : IHandleMessages<PocketBoss.Messages.Commands.RecordStateAction>
    {
        private IBus _bus { get; set; }
        public RecordStateActionHandler(IBus bus)
        {
            _bus = bus;
        }
        public void Handle(Messages.Commands.RecordStateAction message)
        {
            var processor = new ProcessWorkflowRequests(new NServiceBusService(_bus));
            processor.RecordStateActionHandler(message);
        }
    }

    public class RecordTaskActionHandler : IHandleMessages<PocketBoss.Messages.Commands.RecordTaskAction>
    {
        private IBus _bus { get; set; }
        public RecordTaskActionHandler(IBus bus)
        {
            _bus = bus;
        }
        public void Handle(Messages.Commands.RecordTaskAction message)
        {
            var processor = new ProcessWorkflowRequests(new NServiceBusService(_bus));
            processor.RecordTaskActionHandler(message);
        }
    }
}
