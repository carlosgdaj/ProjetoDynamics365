using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;


namespace Plugin
{
    public class ItensPreOperationCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.MessageName.ToLower() == "create" && context.Mode == Convert.ToInt32(MeuEnum.Mode.Synchronous) &&
                context.Stage == Convert.ToInt32(MeuEnum.Stage.PreOperation))
            {
                var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                var serviceUser = serviceFactory.CreateOrganizationService(context.UserId);
                var trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                trace.Trace("Inicio Plugin");
                Entity entidadeContexto = null;

                if (context.InputParameters.Contains("Target"))
                    entidadeContexto = (Entity)context.InputParameters["Target"];

                var itens = Guid.Empty;
                var pedido = Guid.Empty;
                if (entidadeContexto.Attributes.Contains("tcc_itens") && entidadeContexto.Attributes.Contains("tcc_numeropedidoitem"))
                {
                    itens = ((EntityReference)entidadeContexto.Attributes["tcc_itens"]).Id;

                    pedido = ((EntityReference)entidadeContexto.Attributes["tcc_numeropedidoitem"]).Id;

                    trace.Trace("tcc_itens: " + itens);
                    trace.Trace("tcc_numeropedidoitem: " + pedido);
                    QueryExpression queryExpression = new QueryExpression("tcc_listadeitens");

                    queryExpression.Criteria.AddCondition("tcc_itens", ConditionOperator.Equal, itens);
                    queryExpression.Criteria.AddCondition("tcc_numeropedidoitem", ConditionOperator.Equal, pedido);
                    queryExpression.ColumnSet = new ColumnSet("tcc_itens", "tcc_numeropedidoitem");
                    var colecaoEntidades = serviceUser.RetrieveMultiple(queryExpression);
                    trace.Trace("teste: " + colecaoEntidades.Entities.Count);
                    if (colecaoEntidades.Entities.Count > 0)
                        throw new InvalidPluginExecutionException("Item já cadastrado!");
                }
            }
        }

    }
}