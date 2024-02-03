using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;

namespace Ahsoka.Application.Common.Models;

public record Notifier
{
    public Notifier()
    {
        Warnings = [];
        Errors = [];
    }

    public List<ErrorModel> Warnings { get; private set; }
    public List<ErrorModel> Errors { get; private set; }
}
