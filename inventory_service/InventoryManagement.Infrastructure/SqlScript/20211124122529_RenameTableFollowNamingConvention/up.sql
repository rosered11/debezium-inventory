START TRANSACTION;

ALTER TABLE "OutboxEvent" DROP CONSTRAINT "PK_OutboxEvent";

ALTER TABLE "OutboxEvent" RENAME TO outboxevent;

ALTER TABLE "Idempotent" RENAME TO idempotent;

ALTER TABLE "Inventories" RENAME TO inventory;

ALTER TABLE outboxevent ADD CONSTRAINT "PK_outboxevent" PRIMARY KEY (id);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20211124122529_RenameTableFollowNamingConvention', '5.0.11');

COMMIT;

