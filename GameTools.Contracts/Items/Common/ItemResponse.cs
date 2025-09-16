namespace GameTools.Contracts.Items.Common
{
    public sealed record ItemResponse(
        int Id, string Name, int Price, string? Description,
        byte RarityId, string RarityGrade, string RarityColorCode,
        string RowVersionBase64
    );
}
