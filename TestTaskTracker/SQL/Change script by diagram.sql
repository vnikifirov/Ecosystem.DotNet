/*
   1 февраля 2022 г.22:27:39
   User: 
   Server: (localdb)\mssqllocaldb
   Database: Tasks
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Projects SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Tasks ADD CONSTRAINT
	FK_Tasks_Projects FOREIGN KEY
	(
	Id_Project
	) REFERENCES dbo.Projects
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Tasks SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
