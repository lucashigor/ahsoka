﻿using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;

namespace Ahsoka.Application.Administrations.Configurations.Errors;

public static class ConfigurationApplicationError
{
    public static ErrorModel Validation = new(1000, "");
}
