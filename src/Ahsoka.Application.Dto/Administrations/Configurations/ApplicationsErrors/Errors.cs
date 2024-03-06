using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;

namespace Ahsoka.Application.Dto.Administrations.Configurations.ApplicationsErrors
{
    public sealed record ConfigurationErrorCodes : ApplicationErrorCode
    {
        private ConfigurationErrorCodes(int original) : base(original)
        {
        }
        public static readonly ConfigurationErrorCodes NotFound = new(11_000);
        public static readonly ConfigurationErrorCodes ErrorOnSavingNewConfiguration = new(11_001);
        public static readonly ConfigurationErrorCodes ConfigurationsValidation = new(11_002);
        public static readonly ConfigurationErrorCodes ThereWillCurrentConfigurationStartDate = new(11_003);
        public static readonly ConfigurationErrorCodes ThereWillCurrentConfigurationEndDate = new(11_004);
        public static readonly ConfigurationErrorCodes StartDateCannotBeBeforeUtcNow = new(11_005);
        public static readonly ConfigurationErrorCodes EndDateCannotBeBeforeToToday = new(11_006);
        public static readonly ConfigurationErrorCodes OnlyDescriptionAreAvailableToChangedOnClosedConfiguration = new(11_007);
        public static readonly ConfigurationErrorCodes ItsNotAllowedToChangeFinalDateToBeforeToday = new(11_008);
        public static readonly ConfigurationErrorCodes ItsNotAllowedToChangeInitialDate = new(11_009);
        public static readonly ConfigurationErrorCodes ItsNotAllowedToChangeName = new(11_010);
        public static readonly ConfigurationErrorCodes TheMinimumDurationIsOneHour = new(11_011);
        public static readonly ConfigurationErrorCodes ConfigurationInCourse = new(11_012);
        public static readonly ConfigurationErrorCodes ThisCannotBeDoneOnClosedConfiguration = new(11_013);
    }
}

namespace Ahsoka.Application.Dto.Common.ApplicationsErrors
{
    using Ahsoka.Application.Dto.Administrations.Configurations.ApplicationsErrors;
    public partial record Errors
    {
        public static ErrorModel ConfigurationNotFound() => new(ConfigurationErrorCodes.NotFound, "Configuration Not Found.");
        public static ErrorModel ConfigurationValidation() => new(ConfigurationErrorCodes.ConfigurationsValidation, "Configuration Validation.");
        public static ErrorModel ErrorOnSavingNewConfiguration() => new(ConfigurationErrorCodes.ErrorOnSavingNewConfiguration, "Unfortunately an error occurred when saving the Configuration.");
        public static ErrorModel ThereWillCurrentConfigurationStartDate() => new(ConfigurationErrorCodes.ThereWillCurrentConfigurationStartDate, "There will be a current configuration on the start date.");
        public static ErrorModel ThereWillCurrentConfigurationEndDate() => new(ConfigurationErrorCodes.ThereWillCurrentConfigurationEndDate, "There will be a current configuration on the end date.");
        public static ErrorModel StartDateCannotBeBeforeToUtcNow() => new(ConfigurationErrorCodes.StartDateCannotBeBeforeUtcNow, "The start date cannot be before UTC now.");
        public static ErrorModel EndDateCannotBeBeforeToToday() => new(ConfigurationErrorCodes.EndDateCannotBeBeforeToToday, "The end date cannot be before today.");
        public static ErrorModel OnlyDescriptionAreAvailableToChangedOnClosedConfiguration() => new(ConfigurationErrorCodes.OnlyDescriptionAreAvailableToChangedOnClosedConfiguration, "Only description are available to be changed on closed configuration.");
        public static ErrorModel ItsNotAllowedToChangeFinalDateToBeforeToday() => new(ConfigurationErrorCodes.ItsNotAllowedToChangeFinalDateToBeforeToday, "It's not allowed to change final date to before today.");
        public static ErrorModel ItsNotAllowedToChangeInitialDate() => new(ConfigurationErrorCodes.ItsNotAllowedToChangeInitialDate, "It's not allowed to change initial date on configurations in course.");
        public static ErrorModel ItsNotAllowedToChangeName() => new(ConfigurationErrorCodes.ItsNotAllowedToChangeName, "It's not allowed to change name on configurations in course.");
        public static ErrorModel TheMinimumDurationIsOneHour() => new(ConfigurationErrorCodes.TheMinimumDurationIsOneHour, "The minimum duration is one hour.");
        public static ErrorModel ConfigurationInCourse() => new(ConfigurationErrorCodes.ConfigurationInCourse, "This configuration already initiated, so the can't be deleted.");
        public static ErrorModel ThisCannotBeDoneOnClosedConfiguration() => new(ConfigurationErrorCodes.ThisCannotBeDoneOnClosedConfiguration, "This cannot be done on closed configuration.");
    }

}