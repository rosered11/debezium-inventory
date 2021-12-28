CREATE DATABASE "inventory" WITH ENCODING 'UTF8' OWNER postgres;

CREATE USER usr_inventory WITH PASSWORD 'strong_password';

GRANT TEMP ON DATABASE "inventory" TO usr_inventory;

-- Run on database inventory
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO usr_inventory;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO usr_inventory;
GRANT USAGE ON ALL SEQUENCES IN SCHEMA public TO usr_inventory;