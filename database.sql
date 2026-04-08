CREATE LOGIN app_login WITH PASSWORD = 'your_new_password';
GRANT CONNECT SQL TO app_login;

CREATE DATABASE AuthRegistry;
USE AuthRegistry;

CREATE USER auth_app_login FOR LOGIN app_login;

GRANT SELECT, INSERT ON OBJECT::dbo.Logs TO auth_app_login;
GRANT SELECT ON OBJECT::dbo.Roles TO auth_app_login; 
GRANT SELECT ON OBJECT::dbo.Factors TO auth_app_login;
GRANT SELECT, INSERT, UPDATE ON OBJECT::dbo.Users TO auth_app_login;
GRANT SELECT, INSERT, DELETE ON OBJECT::dbo.UserRoles TO auth_app_login;
GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::dbo.UserTokens TO auth_app_login;
GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::dbo.UserResetCodes TO auth_app_login;
GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::dbo.SecondFactorCodes TO auth_app_login;

CREATE TABLE dbo.Logs(
    log_id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
    log_uuid uniqueidentifier NOT NULL DEFAULT NEWID(),
    log_text varchar(100) NOT NULL,
    log_date datetime NOT NULL
);
CREATE TABLE dbo.Roles(
    role_id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
    role_name varchar(20) NOT NULL
);
CREATE TABLE dbo.Factors(
    factor_id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
    factor_name varchar(10) NOT NULL
);
CREATE TABLE dbo.Users(
    user_id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
    uuid uniqueidentifier NOT NULL DEFAULT NEWID(),
    name_user varchar(50) NOT NULL UNIQUE,
    email varchar(50) NOT NULL UNIQUE,
    phone varchar(13) NOT NULL,
    password_hash varchar(64) NOT NULL,
    salt_char varchar(16) NOT NULL,
    second_auth int FOREIGN KEY REFERENCES Factors(factor_id) NOT NULL,
    is_active bit NOT NULL DEFAULT 1,
    created_at datetime NOT NULL DEFAULT SYSDATETIME()
);
CREATE TABLE dbo.UserRoles(
    user_id int FOREIGN KEY REFERENCES Users(user_id) NOT NULL,
    role_id int FOREIGN KEY REFERENCES Roles(role_id) NOT NULL
);
CREATE TABLE dbo.UserTokens(
    token_id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
    user_id int FOREIGN KEY REFERENCES Users(user_id) NOT NULL,
    token_hash varchar(64) NOT NULL,
    refresh datetime NOT NULL,
    expires datetime NOT NULL,
    revoked bit NOT NULL DEFAULT 0
);
CREATE TABLE dbo.UserResetCodes(
    code_id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
    user_id int FOREIGN KEY REFERENCES Users(user_id) NOT NULL,
    code varchar(6) NOT NULL,
    expires datetime NOT NULL,
    used bit NOT NULL DEFAULT 0,
    times_used tinyint NOT NULL DEFAULT 0
);

CREATE TABLE dbo.SecondFactorCodes(
    code_id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
    user_id int FOREIGN KEY REFERENCES Users(user_id) NOT NULL,
    code varchar(6) NOT NULL,
    expires datetime NOT NULL,
    used bit NOT NULL DEFAULT 0,
    times_used tinyint NOT NULL DEFAULT 0
);

INSERT dbo.Roles(role_name) VALUES 
    ('GiveRole'), ('RemoveRole'), ('ReadLogs'), ('LockUser'), ('UnlockUser'),
    ('Role1'), ('Role2'), ('Role3');

INSERT dbo.Factors(factor_name) VALUES
    ('Phone'), ('Email');

-- Replace with your email and phone, the password is: Admin
INSERT dbo.Users(name_user, email, phone, password_hash, salt_char, second_auth) VALUES
    ('Admin', 'admin@email.com', '0000000000000',
    'd6ed08e1a262c1003667ddfba91d41fd740205f7f6e700cb14b9471b228f573a',
    'l79a33fgt7fhp98d', 1);