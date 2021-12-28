START TRANSACTION;

ALTER TABLE "Inventories" DROP COLUMN "Uom";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20211116055943_AddColumnUomOnInventory';

COMMIT;

