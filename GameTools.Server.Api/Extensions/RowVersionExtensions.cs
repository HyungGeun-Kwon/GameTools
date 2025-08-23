namespace GameTools.Server.Api.Extensions
{
    internal static class RowVersionExtensions
    {
        internal static byte[] FromBase64RowVersion(this string base64)
            => Convert.FromBase64String(base64);

        internal static string ToBase64RowVersion(this byte[] rowVersion)
            => Convert.ToBase64String(rowVersion);
    }
}
