using Ahsoka.Domain.Common.ValuesObjects;
using Ahsoka.Domain.Exceptions;
using System.Collections.Immutable;
using Xunit;

namespace Ahsoka.Unit.Tests.Domain.Exceptions;

public class InvalidDomainExceptionTests
{
    [Fact]
    public void Constructor_SetsMessageAndNotifications()
    {
        // Arrange
        var notifications = new List<Notification>
        {
            new Notification("FieldName", "Error Message", DomainErrorCode.Validation),
            new Notification("AnotherField", "Another Error", DomainErrorCode.InvalidYear)
        };

        // Act
        var exception = new InvalidDomainException("Exception Message", notifications);

        // Assert
        Assert.Equal("Exception Message", exception.Message);
        Assert.Equal(notifications.ToImmutableArray(), exception.Notifications);
    }
}
