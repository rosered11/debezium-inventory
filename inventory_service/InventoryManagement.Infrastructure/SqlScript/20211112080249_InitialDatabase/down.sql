START TRANSACTION;

DROP TABLE "Inventories";

DROP TABLE "OutboxEvent";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20211112080249_InitialDatabase';

COMMIT;

