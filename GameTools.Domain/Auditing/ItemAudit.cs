using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameTools.Domain.Auditing
{
    public class ItemAudit : AuditBase<long>
    {
        public int ItemId { get; set; }
        public int ItemIdSnapShot { get; set; }

        private ItemAudit() { } // EF Core

        private ItemAudit(
            AuditAction action,
            int itemId,
            int itemIdSnapshot,
            string? beforeJson,
            string? afterJson,
            string changedBy,
            DateTime? changedAtUtc = null)
            : base(action, beforeJson, afterJson, changedBy, changedAtUtc)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(itemId);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(itemIdSnapshot);

            ItemId = itemId;
            ItemIdSnapShot = itemIdSnapshot;
        }
        // INSERT: AfterJson만 필요
        public static ItemAudit ForInsert(
            int itemId,
            object afterState,
            string changedBy = "system",
            DateTime? changedAtUtc = null,
            JsonSerializerOptions? jsonOptions = null)
            => new(
                action: AuditAction.Insert,
                itemId: itemId,
                itemIdSnapshot: itemId,
                beforeJson: null,
                afterJson: Serialize(afterState, jsonOptions),
                changedBy: changedBy,
                changedAtUtc: changedAtUtc
            );

        // UPDATE: Before/After 모두 필요
        public static ItemAudit ForUpdate(
            int itemId,
            object beforeState,
            object afterState,
            string changedBy = "system",
            DateTime? changedAtUtc = null,
            JsonSerializerOptions? jsonOptions = null)
            => new(
                action: AuditAction.Update,
                itemId: itemId,
                itemIdSnapshot: itemId,
                beforeJson: Serialize(beforeState, jsonOptions),
                afterJson: Serialize(afterState, jsonOptions),
                changedBy: changedBy,
                changedAtUtc: changedAtUtc
            );

        // DELETE: BeforeJson만 필요
        public static ItemAudit ForDelete(
            int itemId,
            object beforeState,
            string changedBy = "system",
            DateTime? changedAtUtc = null,
            JsonSerializerOptions? jsonOptions = null)
            => new(
                action: AuditAction.Delete,
                itemId: itemId,
                itemIdSnapshot: itemId,
                beforeJson: Serialize(beforeState, jsonOptions),
                afterJson: null,
                changedBy: changedBy,
                changedAtUtc: changedAtUtc
            );

        // 이미 직렬화된 JSON을 갖고 있을 때용 오버로드
        public static ItemAudit ForInsertJson(int itemId, string afterJson, string changedBy = "system", DateTime? changedAtUtc = null)
            => new(AuditAction.Insert, itemId, itemId, null, afterJson, changedBy, changedAtUtc);

        public static ItemAudit ForUpdateJson(int itemId, string beforeJson, string afterJson, string changedBy = "system", DateTime? changedAtUtc = null)
            => new(AuditAction.Update, itemId, itemId, beforeJson, afterJson, changedBy, changedAtUtc);

        public static ItemAudit ForDeleteJson(int itemId, string beforeJson, string changedBy = "system", DateTime? changedAtUtc = null)
            => new(AuditAction.Delete, itemId, itemId, beforeJson, null, changedBy, changedAtUtc);
    }
}
