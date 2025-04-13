using System.Text;
using WebApplication2.Services;
using Xunit;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestProjectAdsPlatform
{
    public class UnitTest1
    {
        [Fact]
        public async Task LoadFromFileAsync_ParsesValidData_Correctly()
        {
            // Arrange
            var content = "Яндекс.Директ:/ru\nКрутая реклама:/ru/svrd";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var service = new AdPlatformService();

            // Act
            await service.LoadFromFileAsync(stream);
            var result = service.FindMatchingPlatforms("/ru/svrd");

            // Assert
            Assert.Contains("Яндекс.Директ", result);
            Assert.Contains("Крутая реклама", result);
        }

        [Fact]
        public async Task LoadFromFileAsync_IgnoresInvalidLines()
        {
            var content = "Некорректная строка\nГазета:/ru/msk";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var service = new AdPlatformService();

            await service.LoadFromFileAsync(stream);
            var result = service.FindMatchingPlatforms("/ru/msk");

            Assert.Single(result);
            Assert.Contains("Газета", result);
        }

        [Fact]
        public async Task FindMatchingPlatforms_ReturnsNestedMatches()
        {
            var content = "Яндекс.Директ:/ru\nРевдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var service = new AdPlatformService();

            await service.LoadFromFileAsync(stream);
            var result = service.FindMatchingPlatforms("/ru/svrd/revda");

            Assert.Contains("Яндекс.Директ", result);
            Assert.Contains("Ревдинский рабочий", result);
        }

        [Fact]
        public async Task FindMatchingPlatforms_NoMatches_ReturnsEmpty()
        {
            var content = "Газета:/ru/msk";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var service = new AdPlatformService();

            await service.LoadFromFileAsync(stream);
            var result = service.FindMatchingPlatforms("/en/us/ny");

            Assert.Empty(result);
        }

        [Fact]
        public async Task FindMatchingPlatforms_HandlesSpecialCharactersGracefully()
        {
            var content = "Газета:/ru/msk\n!!!Некорректная:/\\x#$@&\nКрутая:/ru";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var service = new AdPlatformService();

            await service.LoadFromFileAsync(stream);
            var result = service.FindMatchingPlatforms("/ru/msk");

            Assert.Contains("Газета", result);
            Assert.Contains("Крутая", result);
        }
    }
}
