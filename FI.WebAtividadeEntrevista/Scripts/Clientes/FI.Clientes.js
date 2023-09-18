
$(document).ready(function () {
    $('#formCadastro').submit(function (e) {
        e.preventDefault();
        if (ValidaCPF($(this).find("#CPF").val())) {
            $('.btn-success')[0].setAttribute('disabled', true);
            $.ajax({
                url: urlPost,
                method: "POST",
                data: {
                    "NOME": $(this).find("#Nome").val(),
                    "CEP": $(this).find("#CEP").val(),
                    "Email": $(this).find("#Email").val(),
                    "Sobrenome": $(this).find("#Sobrenome").val(),
                    "Nacionalidade": $(this).find("#Nacionalidade").val(),
                    "Estado": $(this).find("#Estado").val(),
                    "Cidade": $(this).find("#Cidade").val(),
                    "Logradouro": $(this).find("#Logradouro").val(),
                    "Telefone": $(this).find("#Telefone").val(),
                    "CPF": $(this).find("#CPF").val(),
                    "Beneficiarios": beneficiarios
                },
                error:
                    function (r) {
                        if (r.status == 400)
                            ModalDialog("Ocorreu um erro", r.responseJSON);
                        else if (r.status == 500)
                            ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");

                        $('.btn-success')[0].removeAttribute('disabled');
                    },
                success:
                    function (r) {
                        $('.btn-success')[0].removeAttribute('disabled');
                        ModalDialog("Sucesso!", r)
                        $("#formCadastro")[0].reset();
                    }
            });

        }
        else {
            $("#cpfError").text("CPF Inválido");
            $("#cpfError").show();
            $('#CPF').addClass('formError');
        }
    });
})

$('#CPF').keyup(function (event) {
    var cpfV = $('#CPF').val();

    cpfV = cpfV.replace(/\D/g, "").replace(/(\d{3})(\d)/, "$1.$2").replace(/(\d{3})(\d)/, "$1.$2").replace(/(\d{3})(\d{1,2})$/, "$1-$2");
    $('#CPF').val(cpfV);
    if (!ValidaCPF(cpfV) && cpfV.length == 14) {
        $("#cpfError").text("CPF Inválido");
        $("#cpfError").show();
        $('#CPF').addClass('formError');
    } else {
        $("#cpfError").hide();
        $('#CPF').removeClass('formError');
    }
})

$('#CEP').keyup(function () {
    var cepV = $('#CEP').val();
    cepV = cepV.replace(/\D/g, "").replace(/(\d{5})(\d)/, "$1-$2");
    $('#CEP').val(cepV);
});

$("#Telefone").keyup(function () {
    var telV = $("#Telefone").val().replace(/\D/g, "");
    telV = telV.replace(/^(\d\d)(\d)/g, "($1) $2");

    if (telV[5] == "9") {
        telV = telV.replace(/(\d{5})(\d)/, "$1-$2")
    } else {
        telV = telV.replace(/(\d{4})(\d)/, "$1-$2")
    }

    $("#Telefone").val(telV);

});

$("#cpfBeneficiario").keyup(function (event) {
    var cpfV = $('#cpfBeneficiario').val();

    cpfV = cpfV.replace(/\D/g, "").replace(/(\d{3})(\d)/, "$1.$2").replace(/(\d{3})(\d)/, "$1.$2").replace(/(\d{3})(\d{1,2})$/, "$1-$2");
    $('#cpfBeneficiario').val(cpfV);
    if (!ValidaCPF(cpfV) && cpfV.length == 14) {
        $("#cpfBeneficiarioError").text("CPF Inválido");
        $("#cpfBeneficiarioError").show()
        $('#cpfBeneficiario').addClass('formError');
    } else {
        $('#cpfBeneficiario').removeClass('formError');
        $("#cpfBeneficiarioError").hide();
    }
})

$("#addBeneficiario").click(function (e) {
    var form = $("#formBeneficiario");
    var cpf = form.find("#cpfBeneficiario").val();
    var nome = form.find("#nomeBeneficiario").val();
    var cpfExists = false;

    if (!ValidaCPF(cpf)) {
        alert("CPF Inválido");
        return;
    } else {
    }

    if (!nome || nome === "") {
        alert("Nome inválido");
        return;
    }

    if (beneficiarios.length > 0) {
        const r = beneficiarios.find((item) => item.cpf == cpf);
        cpfExists = (r != undefined || r != null) || cpf.includes($("#CPF").val());
    } else {
        cpfExists = cpf.includes($("#CPF").val());
    }

    if (!cpfExists) {
        var objBeneficiario = {
            id: 0,
            cpf: cpf,
            nome: nome
        }
        beneficiarios.push(objBeneficiario);
        CarregarTabela();
        form[0].reset();
    }
    else {
        alert("CPF já cadastrado");
    }
});

function ValidaCPF(cpf) {
    var cpfLimpo = cpf.replace(/\D/g, "");
    if (cpfLimpo.length < 11) return false;
    if (cpfLimpo == cpfLimpo[0].toString().repeat(11)) return false;

    var pDigito = returnaDigitoValidador(cpfLimpo, 9)
    if (pDigito != parseInt(cpfLimpo[9])) return false;

    var sDigito = returnaDigitoValidador(cpfLimpo, 10)
    if (sDigito != parseInt(cpfLimpo[10])) return false;

    return true;
}

function returnaDigitoValidador(str, limite) {
    var soma = 0;
    var mod = 0;

    for (var i = 0; i < limite; i++) {
        soma += parseInt(str[i]) * ((limite + 1) - i);
    }

    mod = (soma * 10) % 11;

    mod = mod == 10 || mod == 11 ? 0 : mod;
    return mod;
}

function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                            ';

    $('body').append(texto);
    $('#' + random).modal('show');
}


function ModalBeneficiario() {
    $("#formBeneficiario")[0].reset();
    $('#modalBeneficiarios').modal('show');
    CarregarTabela();
}

function CarregarTabela() {
    $("#tBeneficiarios").empty();
    beneficiarios.forEach((item, index) => {
        var tr = document.createElement("tr");
        var tdNome = document.createElement("td");
        var tdCPF = document.createElement("td");
        var tdBotoes = document.createElement("td");
        var bAlt = document.createElement("button");
        var bDel = document.createElement("button");

        tr.id = item.id;

        tdNome.innerHTML = item.nome;
        tdCPF.innerHTML = item.cpf;

        bAlt.innerHTML = "Alterar";
        bAlt.classList.add("btn", "btn-sm", "btn-info");
        bAlt.setAttribute("type", "button");
        bAlt.setAttribute("onclick", `editarBeneficiario(${index})`);

        bDel.innerHTML = "Excluir";
        bDel.classList.add("btn", "btn-sm", "btn-danger");
        bDel.setAttribute("type", "button");
        bDel.setAttribute("onclick", `removerBeneficiario(${index})`);

        tdBotoes.classList.add("btn-group");
        tdBotoes.appendChild(bAlt);
        tdBotoes.appendChild(bDel);

        tr.appendChild(tdCPF);
        tr.appendChild(tdNome);
        tr.appendChild(tdBotoes);

        $("#tBeneficiarios").append(tr);
    })

}

function editarBeneficiario(index) {
    var beneficiario = beneficiarios[index];
    beneficiarios.splice(index, 1);
    CarregarTabela();
    $("#idBeneficiario").val(beneficiario.id);
    $("#cpfBeneficiario").val(beneficiario.cpf);
    $("#nomeBeneficiario").val(beneficiario.nome);
}

function removerBeneficiario(index) {
    beneficiarios.splice(index, 1);
    CarregarTabela();
}




var beneficiarios = []