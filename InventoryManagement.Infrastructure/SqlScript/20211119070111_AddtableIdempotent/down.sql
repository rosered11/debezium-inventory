START TRANSACTION;

DROP TABLE "Idempotent";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20211119070111_AddtableIdempotent';

COMMIT;

