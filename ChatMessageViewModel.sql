CREATE DATABASE ChatMessageViewModel
USE ChatMessageViewModel;

CREATE TABLE CMVM
(
	Id int PRIMARY KEY NOT NULL,
	IsUser bit,
	Avatar varchar(max),
	Time smalldatetime,	
	ParentId int
);