function Onchange(executionContext) {

    debugger;
    var formContext = executionContext.getFormContext();
    var cnpj = formContext.getAttribute("tcc_cnpj").getValue();

    Retrieve(cnpj);
}
function Retrieve(cnpj) {
    debugger;
    Xrm.WebApi.retrieveMultipleRecords("account", "?$select=tcc_cnpj&$filter=tcc_cnpj eq '" + cnpj + "'").then(
        function success(results) {
            var quantidade = results.entities.length;
            if (quantidade > 0) {
                Xrm.Navigation.openAlertDialog("CNPJ já cadastrado!");
                return;
            }

        },
        function (error) {
            Xrm.Utility.alertDialog(error.message);
        }
    );
}