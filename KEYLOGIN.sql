CREATE DATABASE KEYLOGIN

USE KEYLOGIN;

CREATE TABLE USERS
(
    id int identity(1,1) PRIMARY KEY,
    username varchar(50) NOT NULL UNIQUE,
    email varchar(100) NOT NULL UNIQUE,
);

CREATE TABLE USER_KEYS
(
    id int identity(1,1) PRIMARY KEY,
    user_id int NOT NULL,
    user_key text NOT NULL,
    FOREIGN KEY (user_id) REFERENCES USERS(id)
);
