# Test Data Processing Cluster using Akka.Net

In this test application, we will be simulating message processing from an external source. We will update the state using those messages. Users can then view the snapshot realtime and request some calculation based on the current states.

## Project Structure

The project are consisted of the following components.

- **Lighthouse** - An instance in the Akka Cluster, acting as the seed node to introduce different nodes to each other. It also make decision on which nodes are still available in the cluster. It is assumed to be always on throughout the simulation.

- **MessagePublisher** - Simulating an external component updating its internal states and publishing the snapshots and updates. It is designed so that if the messages is not drained fast enough, the external system will break. IT will call me at midnight to fix the application if somehow there are no instances of `MessageReceiver` running. I have cheated by simulating the component using the same Akka cluster but in reality the messages would come from another source. It is assumed to be always on throughout the simulation.

- **MessagePublisher.Shared** - Library provided by the external component to receive the messages provided by them. We will need to create a child `MessageReceiver` actor and the parent actor will be able to receive all messages from all the defined queues.

```csharp
private void ReceiveMessage()
{
    Receive<IEnumerable<IPublisherMessage>>(messages =>
    {
        //Whatever you want to do with the messages
        Sender.Tell(Ack.Instance);
    });
    Receive<Init>(_ =>
    {
        Sender.Tell(Ack.Instance);
    });
}

protected override void PreStart()
{
    _messageReceiver = Context.ActorOf(Props.Create(() => new MessageReceiver(Self,
        _config)), "message-receiver");
    base.PreStart();
}
```

- **DataReceiver** - The actual application we need to run in order to process the data. Multiple nodes of this application will be started to distribute the workload and also ensure high availability. We will assume only one instance of `MessageReceiver` is running and therefore it would be a singleton in the cluster. Those nodes including the one running the `MessageReceiver` will down randomly and the system will still run as normal, not missing a message and drain the messages fast enough.

- **DataReceiver.Shared** - The **DataReceiver** should contains the startup and configuration information only. The actual implementation should resides here.