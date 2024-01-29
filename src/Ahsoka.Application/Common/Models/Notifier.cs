using Ahsoka.Application.Dto;

namespace Ahsoka.Application;

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
