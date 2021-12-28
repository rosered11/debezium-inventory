START TRANSACTION;

ALTER TABLE "Inventories" ADD "Uom" text NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20211116055943_AddColumnUomOnInventory', '5.0.11');

COMMIT;

