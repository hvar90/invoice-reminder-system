using Xunit;
using Microsoft.Extensions.Configuration;
using InvoicesApi.Services;

public class EmailServiceTests
{
    [Fact]
    public async Task SendEmailAsync_ShouldThrowException_WhenServerIsInvalid()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?> {
            {"EmailSettings:SenderName", "Test Sender"},
            {"EmailSettings:SenderEmail", "sender@example.com"},
            {"EmailSettings:SmtpServer", "invalid.smtp.server.local"},
            {"EmailSettings:Port", "587"},
            {"EmailSettings:Username", "user"},
            {"EmailSettings:Password", "pass"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var emailService = new EmailService(configuration);

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await emailService.SendEmailAsync("client@example.com", "Test Subject", "Test Body");
        });
    }
}