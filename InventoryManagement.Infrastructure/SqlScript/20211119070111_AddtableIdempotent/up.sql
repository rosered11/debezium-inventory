START TRANSACTION;

CREATE TABLE "Idempotent" (
    "EventId" text NOT NULL,
    "EventType" text NOT NULL,
    CONSTRAINT "PrimaryKey_EventIdAndEventType" PRIMARY KEY ("EventId", "EventType")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20211119070111_AddtableIdempotent', '5.0.11');

COMMIT;

