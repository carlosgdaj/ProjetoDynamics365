using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;


namespace Plugin
{
    public class PedidosPreOperationUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.MessageName.ToLower() == "update" && context.Mode == Convert.ToInt32(MeuEnum.Mode.Synchronous) &&
                context.Stage == Convert.ToInt32(MeuEnum.Stage.PreOperation))
            {
                var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                var serviceUser = serviceFactory.CreateOrganizationService(context.UserId);
                var trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                trace.Trace("Inicio Plugin");
                Entity entidadeContexto = null;

                if (context.InputParameters.Contains("Target"))
                    entidadeContexto = (Entity)context.InputParameters["Target"];

                if (entidadeContexto.Attributes.Contains("tcc_numero"))
                {
                    var numero = entidadeContexto.Attributes["tcc_numero"];
                    trace.Trace("new_numero: " + numero);
                    QueryExpression queryExpression = new QueryExpression("tcc_pedidos");

                    queryExpression.Criteria.AddCondition("tcc_numero", ConditionOperator.Equal, numero);
                    queryExpression.ColumnSet = new ColumnSet("tcc_numero");
                    var colecaoEntidades = serviceUser.RetrieveMultiple(queryExpression);
                    trace.Trace("teste: " + colecaoEntidades.Entities.Count);
                    if (colecaoEntidades.Entities.Count > 0)
                        throw new InvalidPluginExecutionException("Número do pedido já cadastrado!");
                }
            }
        }

    }
}