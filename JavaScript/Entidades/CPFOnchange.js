function Onchange(executionContext) {

    debugger;
    var formContext = executionContext.getFormContext();
    var cpf = formContext.getAttribute("tcc_cpf").getValue();

    Retrieve(cpf);
}
function Retrieve(cpf) {
    debugger;
    Xrm.WebApi.retrieveMultipleRecords("contact", "?$select=tcc_cpf&$filter=tcc_cpf eq '" + cpf + "'").then(
        function success(results) {
            var quantidade = results.entities.length;
            if (quantidade > 0) {
                Xrm.Navigation.openAlertDialog("CPF já cadastrado!");
                return;
            }

        },
        function (error) {
            Xrm.Utility.alertDialog(error.message);
        }
    );
}