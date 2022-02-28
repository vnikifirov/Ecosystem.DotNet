-- Constraint on a column of Projects table 
ALTER TABLE [dbo].[Projects]
ADD CONSTRAINT chk_val_limit_project CHECK (Priority between 1 and 10)
GO

-- Constraint on a column of Tasks table 
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT chk_val_limit_task CHECK (Priority between 1 and 10)
GO
