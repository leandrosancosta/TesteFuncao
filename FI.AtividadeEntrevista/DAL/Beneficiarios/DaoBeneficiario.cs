using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FI.AtividadeEntrevista.DAL.Beneficiarios
{
    internal class DaoBeneficiario : AcessoDados
    {
        /// <summary>
        /// Inclui Beneficiarios de um cliente
        /// </summary>
        /// <param name="beneficiarios">Lista de Objeto de beneficiarios</param>
        internal bool Incluir(List<DML.Beneficiario> beneficiarios)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            foreach (DML.Beneficiario beneficiario in beneficiarios)
            {
                parametros.Clear();
                parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", beneficiario.Nome));
                parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF));
                parametros.Add(new System.Data.SqlClient.SqlParameter("IDCLIENTE", beneficiario.IdCliente));

                base.Executar("FI_SP_IncBeneficiarioV2", parametros);
                
            }
            return true;
        }

        /// <summary>
        /// Listas Beneficiarios de um cliente
        /// </summary>
        /// <param name="idCliente">id do cliente</param>
        internal List<DML.Beneficiario> ListarPorCliente(long idCliente)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter("IDCLIENTE",idCliente));

            DataSet ds = base.Consultar("FI_SP_PesqBeneficiarioPorCliente", parametros);

            List<DML.Beneficiario> lista = Converter(ds);
            return lista;
        }

        /// <summary>
        /// Excluir benefinciario de um cliente
        /// </summary>
        /// <param name="Id">id do beneficiario</param>

        internal void Excluir(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

            base.Executar("FI_SP_DelBeneficiario", parametros);
        }

        internal void Alterar(DML.Beneficiario beneficiario)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("NOME", beneficiario.Nome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF));
            parametros.Add(new System.Data.SqlClient.SqlParameter("ID", beneficiario.Id));

            base.Executar("FI_SP_AltBeneficiario", parametros);
        }

        private List<DML.Beneficiario> Converter(DataSet ds)
        {
            List<DML.Beneficiario> lista = new List<DML.Beneficiario>();

            if(ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DML.Beneficiario beneficiario = new DML.Beneficiario()
                    {
                        Id = dr.Field<long>("ID"),
                        CPF = dr.Field<string>("CPF"),
                        IdCliente = dr.Field<long>("IDCLIENTE"),
                        Nome = dr.Field<string>("NOME")
                    };
                    lista.Add(beneficiario);
                }
            }
            return lista;
        }
    }
}
