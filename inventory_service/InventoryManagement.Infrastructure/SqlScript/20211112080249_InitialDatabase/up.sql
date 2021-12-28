CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Inventories" (
    "PartNo" text NOT NULL,
    "WarehouseLocationNo" text NOT NULL,
    "POQty" integer NOT NULL,
    "ReceivingQty" integer NOT NULL,
    "BalanceQty" integer NOT NULL,
    "RequestQty" integer NOT NULL,
    "AvailableQty" integer NOT NULL,
    "CreatedAt" timestamp with time zone NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "UserCreated" text NULL,
    "UserUpdated" text NULL,
    "UserIdCreated" text NULL,
    "UserIdUpdated" text NULL,
    CONSTRAINT "PrimaryKey_PartNoAndWarehouseLocationNo" PRIMARY KEY ("PartNo", "WarehouseLocationNo")
);

CREATE TABLE "OutboxEvent" (
    id uuid NOT NULL,
    aggregatetype character varying(255) NOT NULL,
    aggregateid character varying(255) NOT NULL,
    type character varying(255) NOT NULL,
    payload jsonb NOT NULL,
    timestamp timestamp with time zone NOT NULL,
    CONSTRAINT "PK_OutboxEvent" PRIMARY KEY (id)
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20211112080249_InitialDatabase', '5.0.11');

COMMIT;

