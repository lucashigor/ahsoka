using Ahsoka.Application.Common.Models;

namespace Ahsoka.Application.Common;

public abstract class BaseCommands
{
    public Notifier _notifier { get; private set; }
    public BaseCommands(Notifier notifier)
    {
        _notifier = notifier;
    }
}
