using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAtividadeEntrevista.Models
{
    public class BeneficiarioModel
    {
        public long Id { get; set; }

        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        public string Nome { get; set; }

        /// <summary>
        /// CPF
        /// </summary>
        [RegularExpression(@"[0-9]{3}[\.][0-9]{3}[\.][0-9]{3}[-][0-9]{2}", ErrorMessage = "Digite um CPF válido"), MinLength(14, ErrorMessage = "Tamanho do CPF inválido"), MaxLength(14, ErrorMessage = "Tamanho do CPF inválido")]
        public string CPF { get; set; }

        /// <summary>
        /// IDCLIENTE
        /// </summary>
        public long IdCliente { get; set; }
    }
}