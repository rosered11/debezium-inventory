CREATE DATABASE "wams-inventory" WITH ENCODING 'UTF8' OWNER postgres;

CREATE USER wams_inventory WITH PASSWORD ')5Kn]U2Z>NM:!W[';

GRANT TEMP ON DATABASE "wams-inventory" TO wams_inventory;

-- Run on database wams-inventory
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO wams_inventory;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO wams_inventory;
GRANT USAGE ON ALL SEQUENCES IN SCHEMA public TO wams_inventory;