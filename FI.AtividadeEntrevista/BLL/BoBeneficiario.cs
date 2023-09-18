using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoBeneficiario
    {
        public bool Incluir(List<DML.Beneficiario> beneficiarios)
        {
            DAL.Beneficiarios.DaoBeneficiario dBen = new DAL.Beneficiarios.DaoBeneficiario();
            return dBen.Incluir(beneficiarios);
        }

        public List<DML.Beneficiario> ListarPorCliente(long idCliente)
        {
            DAL.Beneficiarios.DaoBeneficiario dBen = new DAL.Beneficiarios.DaoBeneficiario();
            return dBen.ListarPorCliente(idCliente);
        }

        public void Excluir(long id)
        {
            DAL.Beneficiarios.DaoBeneficiario dBen = new DAL.Beneficiarios.DaoBeneficiario();
            dBen.Excluir(id);
        }

        public void Alterar(DML.Beneficiario beneficiario)
        {
            DAL.Beneficiarios.DaoBeneficiario dBen = new DAL.Beneficiarios.DaoBeneficiario();
            dBen.Alterar(beneficiario);
        }

    }
}
