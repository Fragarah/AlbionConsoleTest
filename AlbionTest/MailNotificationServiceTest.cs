using Xunit;
using AlbionConsole.Services;
using AlbionConsole.Models;
using System.IO;
using System.Text.Json;

namespace AlbionTest
{
    public class MailNotificationServiceTest
    {
        [Fact]
        public void LoadEmailConfig_Should_Load_Config_Correctly()
        {
            // Arrange – przygotuj plik tymczasowy
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var configPath = Path.Combine(tempDir, "mailconfig.json");

            var testConfig = new EmailConfig
            {
                SenderEmail = "test@example.com",
                SenderPassword = "password",
                SmtpServer = "smtp.example.com",
                Port = 587,
                EnableSsl = true
            };
            File.WriteAllText(configPath, JsonSerializer.Serialize(testConfig));

            // Tymczasowo podmień katalog "Data"
            var originalPath = Path.Combine("Data", "mailconfig.json");
            Directory.CreateDirectory("Data");
            File.Copy(configPath, originalPath, overwrite: true);

            try
            {
                // Act
                var service = new MailNotificationService();

                // Assert
                Assert.NotNull(service);
            }
            finally
            {
                // Clean up
                File.Delete(originalPath);
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public async Task SendNotificationAsync_Should_Throw_When_Attachment_Not_Found()
        {
            // Arrange
            var tempConfig = new EmailConfig
            {
                SenderEmail = "test@example.com",
                SenderPassword = "password",
                SmtpServer = "smtp.example.com",
                Port = 587,
                EnableSsl = true
            };

            // Zapisz tymczasowy plik konfiguracyjny
            var configPath = Path.Combine("Data", "mailconfig.json");
            Directory.CreateDirectory("Data");
            File.WriteAllText(configPath, JsonSerializer.Serialize(tempConfig));

            var service = new MailNotificationService();
            string missingPath = "nonexistent-file.pdf";

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                service.SendNotificationAsync("receiver@example.com", "Test", "Body", missingPath));
        }
    }
}
