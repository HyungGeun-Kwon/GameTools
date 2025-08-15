using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameTools.Domain.Auditing
{
    public class RarityAudit : AuditBase<long>
    {
        public byte RarityId { get; set; }
        public byte RarityIdSnapShot { get; set; }

        private RarityAudit() { } // EF Core

        private RarityAudit(
            AuditAction action,
            byte rarityId,
            byte rarityIdSnapshot,
            string? beforeJson,
            string? afterJson,
            string changedBy,
            DateTime? changedAtUtc = null)
            : base(action, beforeJson, afterJson, changedBy, changedAtUtc)
        {
            if (rarityId == 0) throw new ArgumentOutOfRangeException(nameof(rarityId)); // 0을 미사용 값으로 가정
            if (rarityIdSnapshot == 0) throw new ArgumentOutOfRangeException(nameof(rarityIdSnapshot));

            RarityId = rarityId;
            RarityIdSnapShot = rarityIdSnapshot;
        }

        // INSERT: AfterJson만 필요
        public static RarityAudit ForInsert(
            byte rarityId,
            object afterState,
            string changedBy = "system",
            DateTime? changedAtUtc = null,
            JsonSerializerOptions? jsonOptions = null)
            => new(
                action: AuditAction.Insert,
                rarityId: rarityId,
                rarityIdSnapshot: rarityId,
                beforeJson: null,
                afterJson: Serialize(afterState, jsonOptions),
                changedBy: changedBy,
                changedAtUtc: changedAtUtc
            );

        // UPDATE: Before/After 모두 필요
        public static RarityAudit ForUpdate(
            byte rarityId,
            object beforeState,
            object afterState,
            string changedBy = "system",
            DateTime? changedAtUtc = null,
            JsonSerializerOptions? jsonOptions = null)
            => new(
                action: AuditAction.Update,
                rarityId: rarityId,
                rarityIdSnapshot: rarityId,
                beforeJson: Serialize(beforeState, jsonOptions),
                afterJson: Serialize(afterState, jsonOptions),
                changedBy: changedBy,
                changedAtUtc: changedAtUtc
            );

        // DELETE: BeforeJson만 필요
        public static RarityAudit ForDelete(
            byte rarityId,
            object beforeState,
            string changedBy = "system",
            DateTime? changedAtUtc = null,
            JsonSerializerOptions? jsonOptions = null)
            => new(
                action: AuditAction.Delete,
                rarityId: rarityId,
                rarityIdSnapshot: rarityId,
                beforeJson: Serialize(beforeState, jsonOptions),
                afterJson: null,
                changedBy: changedBy,
                changedAtUtc: changedAtUtc
            );

        // 이미 직렬화된 JSON을 갖고 있을 때용 오버로드
        public static RarityAudit ForInsertJson(byte rarityId, string afterJson, string changedBy = "system", DateTime? changedAtUtc = null)
            => new(AuditAction.Insert, rarityId, rarityId, null, afterJson, changedBy, changedAtUtc);

        public static RarityAudit ForUpdateJson(byte rarityId, string beforeJson, string afterJson, string changedBy = "system", DateTime? changedAtUtc = null)
            => new(AuditAction.Update, rarityId, rarityId, beforeJson, afterJson, changedBy, changedAtUtc);

        public static RarityAudit ForDeleteJson(byte rarityId, string beforeJson, string changedBy = "system", DateTime? changedAtUtc = null)
            => new(AuditAction.Delete, rarityId, rarityId, beforeJson, null, changedBy, changedAtUtc);
    }
}
