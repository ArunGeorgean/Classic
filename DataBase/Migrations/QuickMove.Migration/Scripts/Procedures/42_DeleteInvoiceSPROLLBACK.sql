USE [GlobeDev]
GO
/****** Object:  StoredProcedure [Account].[usp_DeleteInvoice]    Script Date: 07/04/2015 18:46:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Jidheesh>
-- Create date: <Create Date,20 jan 2012,>
-- Description:	<Description,To Delete Invoice>
-- =============================================

ALTER PROCEDURE [Account].[usp_DeleteInvoice](
@ID int
)

AS
BEGIN

-- Start Transaction
BEGIN TRANSACTION
-- Start try Block
BEGIN TRY
Delete Account.InvoiceBankAccountDetails where InvoiceID=@ID 
Delete Account.InvoiceItems where Invoice=@ID 
Delete Account.Invoice where ID=@ID
COMMIT
END TRY
BEGIN CATCH

DECLARE @ErrMsg nvarchar(4000), @ErrSeverity int,@ErrorNumber int
SELECT @ErrMsg =@@error, --ERROR_MESSAGE(),
       @ErrSeverity = ERROR_SEVERITY(),
		@ErrorNumber=ERROR_NUMBER();
  -- Whoops, there was an error
  IF @@TRANCOUNT > 0
     ROLLBACK
  RAISERROR(@ErrMsg, @ErrSeverity, 1)
--print ERROR_MESSAGE()
END CATCH
END
