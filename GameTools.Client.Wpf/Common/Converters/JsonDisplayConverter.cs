using System.Globalization;
using System.Text.Json;

namespace GameTools.Client.Wpf.Common.Converters
{
    public sealed class JsonDisplayConverter : MarkupValueConverterBase
    {
        // 셀 미리보기 길이(0이면 자르지 않음)
        public int MaxPreviewChars { get; set; } = 0;

        // null/빈 문자열일 때 표시할 텍스트
        public string? NullText { get; set; } = "—";

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (string.IsNullOrWhiteSpace(s))
                return NullText ?? string.Empty;

            string pretty;
            try
            {
                using var doc = JsonDocument.Parse(s);
                // JsonElement -> 들여쓰기 적용 문자열
                pretty = JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                // JSON 파싱 실패 시 원문 사용
                pretty = s.Trim();
            }

            if (MaxPreviewChars > 0 && pretty.Length > MaxPreviewChars)
                return pretty.Substring(0, MaxPreviewChars) + " …";

            return pretty;
        }
    }
}
