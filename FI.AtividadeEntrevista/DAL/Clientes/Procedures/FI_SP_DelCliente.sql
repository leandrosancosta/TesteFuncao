﻿CREATE PROC FI_SP_DelCliente
	@ID BIGINT
AS
BEGIN
	DELETE CLIENTES WHERE ID = @ID;
	DELETE BENEFICIARIOS WHERE IDCLIENTE = @ID;
END