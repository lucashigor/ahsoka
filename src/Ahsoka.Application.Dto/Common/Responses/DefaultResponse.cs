namespace Ahsoka.Application.Dto;

public sealed record DefaultResponse<T> where T : class
{
    public DefaultResponse()
    {
        Errors = [];
        Warnings = [];
        Data = null;
        TraceId = string.Empty;
    }

    public DefaultResponse(T data)
        : this()
    {
        Data = data;
    }

    public DefaultResponse(T data, List<ErrorModel> errors, string traceId)
        :this (data)
    {
        Errors.AddRange(errors);
        TraceId = traceId;
    }

    public DefaultResponse(T data, ErrorModel error, string traceId)
        : this(data, [error], traceId)
    { }

    public T? Data { get; set; }
    public string TraceId { get; set; }
    public List<ErrorModel> Errors { get; set; }
    public List<ErrorModel> Warnings { get; set; }
}
