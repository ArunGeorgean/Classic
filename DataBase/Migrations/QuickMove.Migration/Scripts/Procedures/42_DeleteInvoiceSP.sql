

-- =============================================
-- Author:		<Author,,Jidheesh>
-- Create date: <Create Date,20 jan 2012,>
-- Description:	<Description,To Delete Invoice>
-- Description:	<Description,To Test Proc>
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
