namespace GameTools.Client.Domain.Items
{
    public sealed record Item(
        int Id, string Name, int Price, string? Description,
        byte RarityId, string RarityGrade, string RarityColorCode,
        string RowVersionBase64
    );
}
