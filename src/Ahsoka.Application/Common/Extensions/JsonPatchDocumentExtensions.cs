namespace Ahsoka.Application.Common.Extensions;
using Ahsoka.Application.Dto.Common.ApplicationsErrors;
using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;

public static class JSonPatchDocumentExtensions
{
    public static List<ErrorModel> Validate<T>(
        this JsonPatchDocument<T> payload,
        OperationType acceptedOperation,
        List<string> acceptedPaths) where T : class
    {
        var errors = new List<ErrorModel>();

        var operations = payload.Operations.Where(x => x.OperationType == acceptedOperation);

        if (!operations.Any())
        {
            var collection = payload.Operations.Select(x => x.OperationType).ToList();

            collection.ForEach(x => errors.Add(Errors
                    .InvalidOperationOnPatch()
                    .ChangeInnerMessage(x.ToString())));
        }

        if (operations.Any(x => !acceptedPaths.Contains(x.path, StringComparer.OrdinalIgnoreCase)))
        {
            var collection = payload.Operations
                .Where(x => !acceptedPaths.Contains(x.path, StringComparer.OrdinalIgnoreCase))
                .Select(x => x.path)
                .ToList();

            collection.ForEach(x => errors.Add(Errors
                    .InvalidPathOnPatch()
                    .ChangeInnerMessage(x.ToString())));
        }

        return errors;
    }

    public static JsonPatchDocument<R> MapPatchInputToPatchCommand<T, R>(this JsonPatchDocument<T> source) where R : class where T : class
    {
        if (source is null)
        {
            return null!;
        }

        return new(
            source.Operations.Select(operation =>
                new Operation<R>(operation.op, operation.path, operation.from, operation.value)).ToList(),
            source.ContractResolver);
    }
}
