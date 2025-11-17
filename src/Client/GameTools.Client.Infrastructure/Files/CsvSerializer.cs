using System.Globalization;
using System.Reflection;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using GameTools.Client.Application.Ports;


namespace GameTools.Client.Infrastructure.Files
{
    internal class CsvSerializer : ICsvSerializer
    {
        private static CsvConfiguration MakeConfig(CultureInfo? culture)
            => new(culture ?? CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                DetectColumnCountChanges = true,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header?.Trim() ?? string.Empty,
            };

        public async Task WriteAsync<T>(
            Stream destination,
            IEnumerable<T> records,
            IEnumerable<string>? includeProperties = null,
            CultureInfo? culture = null,
            CancellationToken ct = default)
        {
            using var writer = new StreamWriter(destination, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true), leaveOpen: true);
            using var csv = new CsvWriter(writer, MakeConfig(culture));

            var props = ResolveIncludedProps<T>(includeProperties);

            if (props is null)
                await csv.WriteRecordsAsync(records, ct).ConfigureAwait(false);
            else
            {
                foreach (var prop in props) csv.WriteField(prop.Name);
                await csv.NextRecordAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    foreach (var prop in props)
                        csv.WriteField(prop.GetValue(record));

                    await csv.NextRecordAsync().ConfigureAwait(false);
                }
            }

            await writer.FlushAsync(ct).ConfigureAwait(false);
        }

        public async Task WriteTemplateAsync<T>(
            Stream destination,
            IEnumerable<string>? includeProperties = null,
            CultureInfo? culture = null,
            CancellationToken ct = default)
        {
            using var writer = new StreamWriter(destination, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true), leaveOpen: true);
            using var csv = new CsvWriter(writer, MakeConfig(culture));

            var props = ResolveIncludedProps<T>(includeProperties);

            if (props is null)
                csv.WriteHeader<T>();
            else
                foreach (var p in props) csv.WriteField(p.Name);

            await csv.NextRecordAsync().ConfigureAwait(false);
            await writer.FlushAsync(ct).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<T>> ReadAsync<T>(
            Stream source,
            CultureInfo? culture = null,
            CancellationToken ct = default)
        {
            using var reader = new StreamReader(source, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true), leaveOpen: true);
            using var csv = new CsvReader(reader, MakeConfig(culture));

            var list = new List<T>();
            await foreach (var r in csv.GetRecordsAsync<T>(ct).ConfigureAwait(false))
                list.Add(r);

            return list;
        }

        // 포함할 프로퍼티 선택 & 검증
        private static PropertyInfo[]? ResolveIncludedProps<T>(IEnumerable<string>? includeProperties)
        {
            if (includeProperties is null) return null; // 전체 포함

            var names = includeProperties
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (names.Length == 0)
                throw new ArgumentException("At least one property name must be specified when includeProperties is provided.", nameof(includeProperties));

            var allProps = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

            var notFound = names.Where(n => !allProps.ContainsKey(n)).ToArray();
            if (notFound.Length > 0)
                throw new ArgumentException($"Unknown property name(s) for {typeof(T).Name}: {string.Join(", ", notFound)}", nameof(includeProperties));

            // include 순서대로 매칭
            return names.Select(n => allProps[n]).ToArray();
        }
    }
}
