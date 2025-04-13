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
            var content = "������.������:/ru\n������ �������:/ru/svrd";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var service = new AdPlatformService();

            // Act
            await service.LoadFromFileAsync(stream);
            var result = service.FindMatchingPlatforms("/ru/svrd");

            // Assert
            Assert.Contains("������.������", result);
            Assert.Contains("������ �������", result);
        }

        [Fact]
        public async Task LoadFromFileAsync_IgnoresInvalidLines()
        {
            var content = "������������ ������\n������:/ru/msk";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var service = new AdPlatformService();

            await service.LoadFromFileAsync(stream);
            var result = service.FindMatchingPlatforms("/ru/msk");

            Assert.Single(result);
            Assert.Contains("������", result);
        }

        [Fact]
        public async Task FindMatchingPlatforms_ReturnsNestedMatches()
        {
            var content = "������.������:/ru\n���������� �������:/ru/svrd/revda,/ru/svrd/pervik";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var service = new AdPlatformService();

            await service.LoadFromFileAsync(stream);
            var result = service.FindMatchingPlatforms("/ru/svrd/revda");

            Assert.Contains("������.������", result);
            Assert.Contains("���������� �������", result);
        }

        [Fact]
        public async Task FindMatchingPlatforms_NoMatches_ReturnsEmpty()
        {
            var content = "������:/ru/msk";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var service = new AdPlatformService();

            await service.LoadFromFileAsync(stream);
            var result = service.FindMatchingPlatforms("/en/us/ny");

            Assert.Empty(result);
        }

        [Fact]
        public async Task FindMatchingPlatforms_HandlesSpecialCharactersGracefully()
        {
            var content = "������:/ru/msk\n!!!������������:/\\x#$@&\n������:/ru";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var service = new AdPlatformService();

            await service.LoadFromFileAsync(stream);
            var result = service.FindMatchingPlatforms("/ru/msk");

            Assert.Contains("������", result);
            Assert.Contains("������", result);
        }
    }
}
