using System.Text;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public class AdPlatformService
    {
        // In-memory коллекция
        private List<AdPlatform> _platforms = new();

        // Загрузка данных из файла
        public async Task LoadFromFileAsync(Stream fileStream)
        {
            var platforms = new List<AdPlatform>();

            using var reader = new StreamReader(fileStream, Encoding.UTF8);
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                try
                {
                    // Пропуск пустых строк и строк без ':'
                    if (string.IsNullOrWhiteSpace(line) || !line.Contains(':'))
                        continue;

                    var parts = line.Split(':', 2); // ограничиваем split, чтобы ':' в названии не ломал
                    var name = parts[0].Trim();
                    var locationsPart = parts[1];

                    // Парсим локации
                    var locations = locationsPart
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Where(loc => loc.StartsWith("/")) // фильтруем кривые локации
                        .Distinct()
                        .ToList();

                    if (!string.IsNullOrEmpty(name) && locations.Any())
                    {
                        platforms.Add(new AdPlatform
                        {
                            Name = name,
                            Locations = locations
                        });
                    }
                }
                catch
                {
                    // Игнорируем строку, если она вызывает ошибку
                    continue;
                }
            }

            _platforms = platforms;
        }

        // Поиск площадок по вложенности локации
        public List<string> FindMatchingPlatforms(string location)
        {
            if (string.IsNullOrWhiteSpace(location) || !location.StartsWith("/"))
                return new(); // пустой список, если локация некорректна

            var result = new List<string>();

            foreach (var platform in _platforms)
            {
                foreach (var loc in platform.Locations)
                {
                    // Локация пользователя вложена в loc → значит loc глобальнее
                    if (location.StartsWith(loc, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(platform.Name);
                        break; // не добавляем платформу дважды
                    }
                }
            }

            return result;
        }
    }
}
