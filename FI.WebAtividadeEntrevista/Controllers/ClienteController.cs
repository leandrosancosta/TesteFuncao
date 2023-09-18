using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            try
            {
                BoCliente bo = new BoCliente();
                if (!ValidaCPF(model.CPF) || bo.VerificarExistencia(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("CPF Inválido ou já existe");
                }

                if(JsonConvert.SerializeObject(model).Contains("' OR 1=1--"))
                {
                    Response.StatusCode = 400;
                    return Json("Itens inválidos no formulário");
                }


                if (model.Beneficiarios != null && model.Beneficiarios.Count > 0)
                {
                    foreach (BeneficiarioModel beneficiario in model.Beneficiarios)
                    {
                        if (!ValidaCPF(beneficiario.CPF) || beneficiario.CPF.Equals(model.CPF))
                        {
                            return Json($"CPF do beneficiario {beneficiario.Nome} é inválido");
                        }
                    }
                }

                if (!this.ModelState.IsValid)
                {
                    List<string> erros = (from item in ModelState.Values
                                          from error in item.Errors
                                          select error.ErrorMessage).ToList();
                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, erros));
                }
                else
                {
                    model.Id = bo.Incluir(new Cliente()
                    {
                        CEP = model.CEP,
                        Cidade = model.Cidade,
                        Email = model.Email,
                        Estado = model.Estado,
                        Logradouro = model.Logradouro,
                        Nacionalidade = model.Nacionalidade,
                        Nome = model.Nome,
                        Sobrenome = model.Sobrenome,
                        Telefone = model.Telefone,
                        CPF = model.CPF
                    });

                    if (model.Beneficiarios != null && model.Beneficiarios.Count > 0)
                    {
                        BoBeneficiario boBeneficiario = new BoBeneficiario();
                        List<Beneficiario> beneficiarios = new List<Beneficiario>();
                        foreach (BeneficiarioModel beneficiario in model.Beneficiarios)
                        {
                            beneficiarios.Add(new Beneficiario()
                            {
                                CPF = beneficiario.CPF,
                                IdCliente = model.Id,
                                Nome = beneficiario.Nome
                            });

                        }
                        boBeneficiario.Incluir(beneficiarios);
                    }

                    return Json("Cadastro efetuado com sucesso");
                }
            }
            catch (System.Data.SqlClient.SqlException SqlEx)
            {
                Response.StatusCode = 500;
                return Json("Erro no servidor de dados");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json($"Erro interno contate o administrador do sistema: {ex.Message}");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            try
            {
                if (JsonConvert.SerializeObject(model).Contains("' OR 1=1--"))
                {
                    Response.StatusCode = 400;
                    return Json("Itens inválidos no formulário");
                }

                BoCliente bo = new BoCliente();
                Cliente cliente = bo.Consultar(model.Id);

                if (!model.CPF.Equals(cliente.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("Não é possível alterar o CPF");
                }

                if (!this.ModelState.IsValid)
                {
                    List<string> erros = (from item in ModelState.Values
                                          from error in item.Errors
                                          select error.ErrorMessage).ToList();

                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, erros));
                }
                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone
                });
                return Json("Cadastro alterado com sucesso");
            }
            catch (System.Data.SqlClient.SqlException SqlEx)
            {
                Response.StatusCode = 500;
                return Json($"Erro no servidor de dados: {SqlEx.Message}");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json($"Erro interno contate o administrador do sistema: {ex.Message}");
            }

        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            try
            {
                BoCliente bo = new BoCliente();
                Cliente cliente = bo.Consultar(id);
                Models.ClienteModel model = null;

                if (cliente != null)
                {
                    model = new ClienteModel()
                    {
                        Id = cliente.Id,
                        CEP = cliente.CEP,
                        Cidade = cliente.Cidade,
                        Email = cliente.Email,
                        Estado = cliente.Estado,
                        Logradouro = cliente.Logradouro,
                        Nacionalidade = cliente.Nacionalidade,
                        Nome = cliente.Nome,
                        Sobrenome = cliente.Sobrenome,
                        Telefone = cliente.Telefone,
                        CPF = cliente.CPF
                    };

                    BoBeneficiario bBeneficiario = new BoBeneficiario();
                    List<Beneficiario> beneficiarios = bBeneficiario.ListarPorCliente(model.Id);

                    if (beneficiarios != null && beneficiarios.Count > 0)
                    {
                        model.Beneficiarios = new List<BeneficiarioModel>();

                        foreach (Beneficiario beneficiario in beneficiarios)
                        {
                            model.Beneficiarios.Add(new BeneficiarioModel()
                            {
                                CPF = beneficiario.CPF,
                                Id = beneficiario.Id,
                                IdCliente = beneficiario.IdCliente,
                                Nome = beneficiario.Nome
                            });
                        }
                    }
                }

                return View(model);
            }
            catch (System.Data.SqlClient.SqlException SqlEx)
            {
                Response.StatusCode = 500;
                ViewBag.Error = $"Erro no servidor de dados: {SqlEx.Message}";
                return View("~/Views/Shared/Error.cshtml");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                ViewBag.Error = $"Erro no servidor: {ex.Message}";
                return View("~/Views/Shared/Error.cshtml");
            }

        }

        [HttpPost]
        public JsonResult AltIncBeneficiario(BeneficiarioModel beneficiarioModel)
        {
            try
            {
                if (JsonConvert.SerializeObject(beneficiarioModel).Contains("' OR 1=1--"))
                {
                    Response.StatusCode = 400;
                    return Json("Itens inválidos no formulário");
                }

                BoBeneficiario boBeneficiario = new BoBeneficiario();
                Beneficiario beneficiario = boBeneficiario.ListarPorCliente(beneficiarioModel.IdCliente).Where(b => b.CPF.Equals(beneficiarioModel.CPF) ).FirstOrDefault();
                Beneficiario nBeneficiario = new Beneficiario()
                {
                    CPF = beneficiarioModel.CPF,
                    IdCliente = beneficiarioModel.IdCliente,
                    Nome = beneficiarioModel.Nome
                };
                if (beneficiario == null)
                {
                    List<Beneficiario> lista = new List<Beneficiario>();
                    lista.Add(nBeneficiario);
                    boBeneficiario.Incluir(lista);
                }
                else
                {
                    nBeneficiario.Id = beneficiarioModel.Id;
                    boBeneficiario.Alterar(nBeneficiario);
                }
                return Json("Beneficiario Alterado com sucesso");
            }
            catch (System.Data.SqlClient.SqlException SqlEx)
            {
                Response.StatusCode = 500;
                return Json($"Erro no servidor de dados: {SqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Json($"Erro interno contate o administrador do sistema: {ex.Message}");
            }

        }
        [HttpPost]
        public JsonResult ExcluirBeneficiario(long Id)
        {
            try
            {
                BoBeneficiario boBeneficiario = new BoBeneficiario();
                Response.StatusCode = 200;
                boBeneficiario.Excluir(Id);
                return Json("Beneficiario Apagado com sucesso com sucesso");
            }
            catch (System.Data.SqlClient.SqlException SqlEx)
            {
                Response.StatusCode = 500;
                return Json($"Erro no servidor de dados: {SqlEx.Message}");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json($"Erro interno contate o administrador do sistema: {ex.Message}");
            }
        }
        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                if (jtSorting.Contains("' OR 1=1--"))
                {
                    Response.StatusCode = 400;
                    return Json("Itens inválidos na pesquisa");
                }

                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (System.Data.SqlClient.SqlException SqlEx)
            {
                Response.StatusCode = 500;
                return Json("Erro no servidor de dados");
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private bool ValidaCPF(string CPF)
        {
            CPF = Regex.Replace(CPF, "[^0-9a-zA-Z]+", "");
            if (CPF.Length != 11)
                return false;
            string digitoRepetido = new string(CPF[0], 11);
            if (CPF.Equals(digitoRepetido))
                return false;
            int pDigito = RetornaDigito(CPF, 9);
            if (pDigito != int.Parse(CPF[9].ToString())) return false;
            int sDigito = RetornaDigito(CPF, 10);
            if (sDigito != int.Parse(CPF[10].ToString())) return false;
            return true;
        }

        private int RetornaDigito(string cpf, int limite)
        {
            int mod;
            int soma = 0;
            for (int i = 0; i < limite; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * ((limite + 1) - i);
            }

            mod = (soma * 10) % 11;
            mod = mod > 9 ? 0 : mod;

            return mod;
        }
    }
}