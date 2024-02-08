using Ahsoka.Application.Common.Models;

namespace Ahsoka.Application.Common;

public abstract class BaseCommands(Notifier notifier)
{
    protected Notifier _notifier = notifier;
}
