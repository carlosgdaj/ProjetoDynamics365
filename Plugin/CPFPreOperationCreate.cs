using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin
{
    public class CPFContactCreate : IPlugin
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

                //var cpf = new Entity();
                if (entidadeContexto.Attributes.Contains("tcc_cpf"))
                {
                    var cpf = entidadeContexto.Attributes["tcc_cpf"];
                    trace.Trace("tcc_cpf: " + cpf);
                    QueryExpression queryExpression = new QueryExpression("contact");

                    queryExpression.Criteria.AddCondition("tcc_cpf", ConditionOperator.Equal, cpf);
                    queryExpression.ColumnSet = new ColumnSet("tcc_cpf");
                    var colecaoEntidades = serviceUser.RetrieveMultiple(queryExpression);
                    trace.Trace("teste: " + colecaoEntidades.Entities.Count);
                    if (colecaoEntidades.Entities.Count > 0)
                        throw new InvalidPluginExecutionException("CPF já utilizado em outro contato !");
                }
            }
        }

    }
}