namespace Ahsoka.Application;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using Ahsoka.Application.Dto;

public static class JsonPatchDocumentExtensions
{
    public static void Validate<T>(
        this JsonPatchDocument<T> payload,
        OperationType acceptedOperation,
        List<string> acceptedPaths) where T : class
    {
        var operations = payload.Operations.Where(x => x.OperationType == acceptedOperation);

        if (!operations.Any())
        {
            var err = Errors.InvalidOperationOnPatch();

            var collection = payload.Operations.Select(x => x.OperationType).ToList();

            var op = "";

            foreach (var item in collection)
            {
                op += $"{item},";
            }

            err.ChangeInnerMessage(op ?? "");

            throw new BusinessException(err);
        }

        if (operations.Any(x => !acceptedPaths.Contains(x.path, StringComparer.OrdinalIgnoreCase)))
        {
            var err = Errors.InvalidPathOnPatch();

            var collection = payload.Operations
                .Where(x => !acceptedPaths.Contains(x.path, StringComparer.OrdinalIgnoreCase))
                .Select(x => x.path)
                .ToList();

            var op = "";

            foreach (var item in collection)
            {
                op += $"{item},";
            }

            err.ChangeInnerMessage(op ?? "");

            throw new BusinessException(err);
        };
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
