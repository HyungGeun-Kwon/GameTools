using System.Globalization;

namespace GameTools.Client.Application.Ports
{
    public interface ICsvSerializer
    {
        /// <summary>
        /// <typeparamref name="T"/> 컬렉션을 UTF-8 with BOM으로 CSV Write
        /// </summary>
        /// <param name="destination">출력 대상 스트림(이 메서드가 닫지 않음)</param>
        /// <param name="includeProperties">
        /// null이면 모든 공개 읽기 가능한 프로퍼티 포함.
        /// null이 아니면 지정한 이름들만(대소문자 무시, 지정 순서 유지) 포함.
        /// </param>
        /// <param name="culture">숫자/날짜 포맷 등에 사용할 문화권. null이면 InvariantCulture</param>
        Task WriteAsync<T>(
            Stream destination,
            IEnumerable<T> records,
            IEnumerable<string>? includeProperties = null,
            CultureInfo? culture = null,
            CancellationToken ct = default);

        /// <summary>
        /// <typeparamref name="T"/> 타입 기준 UTF-8 with BOM으로 CSV 헤더 Write
        /// </summary>
        /// <param name="destination">출력 대상 스트림(이 메서드가 닫지 않음)</param>
        /// <param name="includeProperties">
        /// null이면 모든 공개 읽기 가능한 프로퍼티 포함.
        /// null이 아니면 지정한 이름들만(대소문자 무시, 지정 순서 유지) 포함.
        /// </param>
        /// <param name="culture">포맷에 사용할 문화권. null이면 InvariantCulture</param>
        Task WriteTemplateAsync<T>(
        Stream destination,
            IEnumerable<string>? includeProperties = null,
            CultureInfo? culture = null,
            CancellationToken ct = default);

        /// <summary>
        /// CSV를 UTF-8 기준으로 읽어 <typeparamref name="T"/> 목록으로 반환.
        /// <param name="culture">파싱에 사용할 문화권. null이면 InvariantCulture</param>
        Task<IReadOnlyList<T>> ReadAsync<T>(
        Stream source,
            CultureInfo? culture = null,
            CancellationToken ct = default);
    }
}
