using EasyNetQ;
using System;
using System.Threading.Tasks;

public class NullBus : IBus
{
    public IPubSub PubSub => throw new NotImplementedException();
    public IRpc Rpc => throw new NotImplementedException();
    public ISendReceive SendReceive => throw new NotImplementedException();
    public IScheduler Scheduler => throw new NotImplementedException();
    public IAdvancedBus Advanced => throw new NotImplementedException();
    public void Dispose() { }
    public Task DisposeAsync() => Task.CompletedTask;
}