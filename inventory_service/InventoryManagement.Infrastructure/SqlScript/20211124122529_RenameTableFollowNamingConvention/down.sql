START TRANSACTION;

ALTER TABLE outboxevent DROP CONSTRAINT "PK_outboxevent";

ALTER TABLE outboxevent RENAME TO "OutboxEvent";

ALTER TABLE idempotent RENAME TO "Idempotent";

ALTER TABLE inventory RENAME TO "Inventories";

ALTER TABLE "OutboxEvent" ADD CONSTRAINT "PK_OutboxEvent" PRIMARY KEY (id);

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20211124122529_RenameTableFollowNamingConvention';

COMMIT;

