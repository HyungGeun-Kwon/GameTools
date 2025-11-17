using GameTools.Client.Application.Ports;

namespace GameTools.Client.Console
{
    public class ConsoleTestActorProvider : IActorProvider
    {
        public string? GetActor() => "Console_Tester";
    }
}
