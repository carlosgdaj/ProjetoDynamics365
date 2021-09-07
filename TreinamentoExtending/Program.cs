using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Net;
using Microsoft.Xrm.Tooling.Connector;
using System.Timers;
//using ConexaoAlternativaExtending;

namespace TCCFiveDev {
    class Program {

        static private Timer timer = new Timer();

        static void Main(string[] args) {

            var serviceproxy = new Conexao().Obter();
            var serviceproxyCliente = new ConexaoCliente().Obter();

            ProcessorManager();
            EuRoboFinal(serviceproxy, serviceproxyCliente);
            Console.WriteLine("/////////////////////////////////Fim da Importação/////////////////////////////////");
            Console.WriteLine(DateTime.Now);

            //Console.WriteLine("Fim da Importação");
            Console.ReadKey();
        }

        #region Robo Corpo
        static void ProcessorManager()
        {
            AdjustTimer(); //Configura o seu Timer(10 em 10 minutos)
            timer.Start(); //Inicia a contagem do Timer.
        }

        static void PararTimer()
        {
            timer.Stop(); //Vc pode chamar em qualquer lugar para parar o timer.
        }

        static private void OnTimeOut(object source, ElapsedEventArgs e)
        {
            try
            {
                var serviceproxy = new Conexao().Obter();
                var serviceproxyCliente = new ConexaoCliente().Obter();
                EuRoboFinal(serviceproxy, serviceproxyCliente);
                Console.WriteLine("/////////////////////////////////Fim da Importação/////////////////////////////////");
                Console.WriteLine(DateTime.Now);
                /*Aqui adicione o seu código para executar a procedure*/
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }
        }

        static private void AdjustTimer()/*Coração da execução onde vc configura o Timer*/
        {
            timer.Interval = 30000; //30 segundos
            timer.AutoReset = true;

            timer.Elapsed += OnTimeOut;//Evento que será executado quando o tempo que vc estipulou estourar (Momento de chamar sua procedure)	   
        }


        static void EuRoboFinal(CrmServiceClient serviceProxy, CrmServiceClient serviceProxyCiente)
        {

            RetornarMultiploClienteEuRoboContaContato(serviceProxy, serviceProxyCiente);
            RetornarMultiploClienteEuRoboItensPedido(serviceProxy, serviceProxyCiente);
            RetornarMultiploClienteEuRoboPedidos(serviceProxy, serviceProxyCiente);
            RetornarMultiploClienteEuRoboLead(serviceProxy, serviceProxyCiente);
            RetornarMultiploClienteEuRoboListadeitens(serviceProxy, serviceProxyCiente);
            

        }

        
        static EntityCollection RetornarMultiploClienteEuRoboContaContato(CrmServiceClient serviceProxy, CrmServiceClient serviceProxyCliente)
        {
            QueryExpression queryExpressionContato = new QueryExpression("contact");

            queryExpressionContato.Criteria.AddCondition("lastname", ConditionOperator.NotNull);
            queryExpressionContato.Criteria.AddCondition("firstname", ConditionOperator.NotNull);
            queryExpressionContato.Criteria.AddCondition("new_cpf", ConditionOperator.NotNull);

            queryExpressionContato.ColumnSet = new ColumnSet("firstname", "lastname", "jobtitle", "new_cpf", "emailaddress1", "telephone1", "mobilephone", "fax", "preferredcontactmethodcode", "parentcustomerid", "address1_postalcode");
            EntityCollection colecaoEntidadesContato = serviceProxyCliente.RetrieveMultiple(queryExpressionContato);

            foreach (var entidade in colecaoEntidadesContato.Entities)
            {
                try
                {

                    var contato = new Entity("contact");
                    Guid registroConta = new Guid();

                    if (entidade.Attributes.Contains("contactid"))
                        contato.Attributes.Add("contactid", entidade.Id);

                    if (entidade.Attributes.Contains("firstname"))
                        contato.Attributes.Add("firstname", entidade.GetAttributeValue<string>("firstname"));

                    if (entidade.Attributes.Contains("lastname"))
                        contato.Attributes.Add("lastname", entidade.GetAttributeValue<string>("lastname"));

                    if (entidade.Attributes.Contains("jobtitle"))
                        contato.Attributes.Add("jobtitle", entidade.GetAttributeValue<string>("jobtitle"));

                    if (entidade.Attributes.Contains("new_cpf"))
                        contato.Attributes.Add("tcc_cpf", entidade.GetAttributeValue<string>("new_cpf"));

                    if (entidade.Attributes.Contains("emailaddress1"))
                        contato.Attributes.Add("emailaddress1", entidade.GetAttributeValue<string>("emailaddress1"));

                    if (entidade.Attributes.Contains("telephone1"))
                        contato.Attributes.Add("telephone1", entidade.GetAttributeValue<string>("telephone1"));

                    if (entidade.Attributes.Contains("mobilephone"))
                        contato.Attributes.Add("mobilephone", entidade.GetAttributeValue<string>("mobilephone"));

                    if (entidade.Attributes.Contains("fax"))
                        contato.Attributes.Add("fax", entidade.GetAttributeValue<string>("fax"));

                    if (entidade.Attributes.Contains("preferredcontactmethodcode"))
                        contato.Attributes.Add("preferredcontactmethodcode", entidade.GetAttributeValue<OptionSetValue>("preferredcontactmethodcode"));

                    if (entidade.Attributes.Contains("address1_postalcode"))
                        contato.Attributes.Add("address1_postalcode", entidade.GetAttributeValue<string>("address1_postalcode"));

                    registroConta = serviceProxy.Create(contato);

                }

                catch (Exception e)
                {

                    Console.WriteLine("{0} Exception caught.", e);

                }

            }

            QueryExpression queryExpressionConta = new QueryExpression("account");

            queryExpressionConta.Criteria.AddCondition("new_cnpj", ConditionOperator.NotNull);
            queryExpressionConta.Criteria.AddCondition("name", ConditionOperator.NotNull);
            queryExpressionConta.ColumnSet = new ColumnSet("name", "telephone1", "fax", "websiteurl", "tickersymbol", "new_cnpj", "address1_postalcode", "primarycontactid", "parentaccountid", "address1_stateorprovince", "address1_city");
            EntityCollection colecaoEntidadesConta = serviceProxyCliente.RetrieveMultiple(queryExpressionConta);

            foreach (var entidade in colecaoEntidadesConta.Entities)
            {
                try

                {
                    var conta = new Entity("account");
                    Guid registroConta = new Guid();

                    if (entidade.Attributes.Contains("accountid"))
                        conta.Attributes.Add("accountid", entidade.Id);

                    if (entidade.Attributes.Contains("name"))
                        conta.Attributes.Add("name", entidade.GetAttributeValue<string>("name"));

                    if (entidade.Attributes.Contains("telephone1"))
                        conta.Attributes.Add("telephone1", entidade.GetAttributeValue<string>("telephone1"));

                    if (entidade.Attributes.Contains("fax"))
                        conta.Attributes.Add("fax", entidade.GetAttributeValue<string>("fax"));

                    if (entidade.Attributes.Contains("websiteurl"))
                        conta.Attributes.Add("websiteurl", entidade.GetAttributeValue<string>("websiteurl"));

                    if (entidade.Attributes.Contains("tickersymbol"))
                        conta.Attributes.Add("tickersymbol", entidade.GetAttributeValue<string>("tickersymbol"));

                    if (entidade.Attributes.Contains("new_cnpj"))
                        conta.Attributes.Add("tcc_cnpj", entidade.GetAttributeValue<string>("new_cnpj"));

                    if (entidade.Attributes.Contains("primarycontactid"))
                        conta.Attributes.Add("primarycontactid", entidade.GetAttributeValue<EntityReference>("primarycontactid"));

                    if (entidade.Attributes.Contains("address1_postalcode"))
                        conta.Attributes.Add("address1_postalcode", entidade.GetAttributeValue<string>("address1_postalcode"));

                    if (entidade.Attributes.Contains("address1_stateorprovince"))
                        conta.Attributes.Add("address1_stateorprovince", entidade.GetAttributeValue<string>("address1_stateorprovince"));

                    if (entidade.Attributes.Contains("address1_city"))
                        conta.Attributes.Add("address1_city", entidade.GetAttributeValue<string>("address1_city"));


                    registroConta = serviceProxy.Create(conta);

                }

                catch (Exception e)
                {

                    Console.WriteLine("{0} Exception caught.", e);

                }


            }

            foreach (var entidade in colecaoEntidadesConta.Entities)
            {
                try
                {
                    var registroDynamics = serviceProxy.Retrieve("account", entidade.Id, new ColumnSet("parentaccountid"));

                    if (entidade.Attributes.Contains("parentaccountid"))
                        registroDynamics.Attributes.Add("parentaccountid", entidade.GetAttributeValue<EntityReference>("parentaccountid"));

                    serviceProxy.Update(registroDynamics);

                }
                catch (Exception e)
                {

                    Console.WriteLine("{0} Exception caught.", e);

                }


            }

            foreach (var entidade in colecaoEntidadesContato.Entities)
            {
                try
                {
                    var registroDynamics = serviceProxy.Retrieve("contact", entidade.Id, new ColumnSet("parentcustomerid"));

                    if (entidade.Attributes.Contains("parentcustomerid"))
                        registroDynamics.Attributes.Add("parentcustomerid", entidade.GetAttributeValue<EntityReference>("parentcustomerid"));

                    serviceProxy.Update(registroDynamics);

                }
                catch (Exception e)
                {

                    Console.WriteLine("{0} Exception caught.", e);

                }


            }


            return colecaoEntidadesConta;

        }

        static EntityCollection RetornarMultiploClienteEuRoboPedidos(CrmServiceClient serviceProxy, CrmServiceClient serviceProxyCliente)
        {
            QueryExpression queryExpression = new QueryExpression("new_pedidos");

            queryExpression.Criteria.AddCondition("new_numero", ConditionOperator.NotNull);


            queryExpression.ColumnSet = new ColumnSet("new_numero", "new_cliente", "new_item", "new_contato", "new_quantidade", "new_valordopedido", "new_previsaodeentrega", "new_enderecodaentrega", "new_cep", "new_valordofrete");
            EntityCollection colecaoEntidades = serviceProxyCliente.RetrieveMultiple(queryExpression);

            foreach (var entidade in colecaoEntidades.Entities)
            {
                try
                {
                    var pedidos = new Entity("tcc_pedido");
                    Guid registroConta = new Guid();

                    if (entidade.Attributes.Contains("new_pedidosid"))
                        pedidos.Attributes.Add("tcc_pedidoid", entidade.Id);

                    if (entidade.Attributes.Contains("new_numero"))
                        pedidos.Attributes.Add("tcc_numerodopedido", entidade.GetAttributeValue<string>("new_numero"));

                    if (entidade.Attributes.Contains("new_cliente"))
                        pedidos.Attributes.Add("tcc_cliente", entidade.GetAttributeValue<EntityReference>("new_cliente"));

                    if (entidade.Attributes.Contains("new_item"))
                        pedidos.Attributes.Add("tcc_item", entidade.GetAttributeValue<EntityReference>("new_item"));

                    if (entidade.Attributes.Contains("new_quantidade"))
                        pedidos.Attributes.Add("tcc_quantidade", entidade.GetAttributeValue<int>("new_quantidade").ToString());

                    if (entidade.Attributes.Contains("new_valordopedido"))
                        pedidos.Attributes.Add("tcc_valordopedido", entidade.GetAttributeValue<int>("new_valordopedido").ToString());

                    if (entidade.Attributes.Contains("new_contato"))
                        pedidos.Attributes.Add("tcc_contato", entidade.GetAttributeValue<EntityReference>("new_contato"));

                    if (entidade.Attributes.Contains("new_previsaodeentrega"))
                        pedidos.Attributes.Add("tcc_previsaodeentrega", entidade.GetAttributeValue<string>("new_previsaodeentrega"));

                    if (entidade.Attributes.Contains("new_enderecodaentrega"))
                        pedidos.Attributes.Add("tcc_enderecodeentrega", entidade.GetAttributeValue<string>("new_enderecodaentrega"));

                    if (entidade.Attributes.Contains("new_cep"))
                        pedidos.Attributes.Add("tcc_cep", entidade.GetAttributeValue<string>("new_cep"));

                    if (entidade.Attributes.Contains("new_valordofrete"))
                        pedidos.Attributes.Add("tcc_frete", entidade.GetAttributeValue<string>("new_valordofrete"));

                    if (entidade.Attributes.Contains("new_pedidosid"))
                        pedidos.Attributes.Add("tcc_chavedeterceiro", entidade.GetAttributeValue<Guid>("new_pedidosid").ToString());


                    registroConta = serviceProxy.Create(pedidos);

                }
                catch (Exception e)
                {

                    Console.WriteLine("{0} Exception caught.", e);

                }


            }

            return colecaoEntidades;

        }

        static EntityCollection RetornarMultiploClienteEuRoboItensPedido(CrmServiceClient serviceProxy, CrmServiceClient serviceProxyCliente)
        {
            QueryExpression queryExpression = new QueryExpression("new_itensdopedido");

            queryExpression.Criteria.AddCondition("new_codigodoitem", ConditionOperator.NotNull);
            queryExpression.Criteria.AddCondition("new_item", ConditionOperator.NotNull);
            queryExpression.ColumnSet = new ColumnSet("new_item", "new_codigodoitem", "new_garantia", "new_pesoliquido", "new_potenciaemwatts", "new_bluetooth", "new_portasusb", "new_wifi", "new_aprovadagua", "new_duracaodabateria");
            EntityCollection colecaoEntidades = serviceProxyCliente.RetrieveMultiple(queryExpression);

            foreach (var entidade in colecaoEntidades.Entities)
            {
                try
                {
                    var itensdopedido = new Entity("tcc_itemdopedido");
                    Guid registroConta = new Guid();

                    if (entidade.Attributes.Contains("new_itensdopedidoid"))
                        itensdopedido.Attributes.Add("tcc_itemdopedidoid", entidade.Id);

                    if (entidade.Attributes.Contains("new_item"))
                        itensdopedido.Attributes.Add("tcc_item", entidade.GetAttributeValue<string>("new_item"));

                    if (entidade.Attributes.Contains("new_codigodoitem"))
                        itensdopedido.Attributes.Add("tcc_codigodoitem", entidade.GetAttributeValue<string>("new_codigodoitem"));

                    if (entidade.Attributes.Contains("new_garantia"))
                        itensdopedido.Attributes.Add("tcc_garantia", entidade.GetAttributeValue<string>("new_garantia"));

                    if (entidade.Attributes.Contains("new_pesoliquido"))
                        itensdopedido.Attributes.Add("tcc_pesokg", entidade.GetAttributeValue<string>("new_pesoliquido"));

                    if (entidade.Attributes.Contains("new_potenciaemwatts"))
                        itensdopedido.Attributes.Add("tcc_potenciawatts", entidade.GetAttributeValue<string>("new_potenciaemwatts"));

                    if (entidade.Attributes.Contains("new_bluetooth"))
                        itensdopedido.Attributes.Add("tcc_bluetooth", entidade.GetAttributeValue<Boolean>("new_bluetooth"));

                    if (entidade.Attributes.Contains("new_portasusb"))
                        itensdopedido.Attributes.Add("tcc_portausb", entidade.GetAttributeValue<Boolean>("new_portasusb"));

                    if (entidade.Attributes.Contains("new_wifi"))
                        itensdopedido.Attributes.Add("tcc_wifi", entidade.GetAttributeValue<Boolean>("new_wifi"));

                    if (entidade.Attributes.Contains("new_aprovadagua"))
                        itensdopedido.Attributes.Add("tcc_aprovadagua", entidade.GetAttributeValue<Boolean>("new_aprovadagua"));

                    if (entidade.Attributes.Contains("new_duracaodabateria"))
                        itensdopedido.Attributes.Add("tcc_duracaodabateria", entidade.GetAttributeValue<string>("new_duracaodabateria"));

                    registroConta = serviceProxy.Create(itensdopedido);
                }
                catch (Exception e)
                {

                    Console.WriteLine("{0} Exception caught.", e);

                }


            }

            return colecaoEntidades;
        }

        static EntityCollection RetornarMultiploClienteEuRoboLead(CrmServiceClient serviceProxy, CrmServiceClient serviceProxyCliente)
        {
            QueryExpression queryExpression = new QueryExpression("new_lead");

            queryExpression.Criteria.AddCondition("new_name", ConditionOperator.NotNull);
            queryExpression.Criteria.AddCondition("new_telefone", ConditionOperator.NotNull);
            queryExpression.Criteria.AddCondition("new_email", ConditionOperator.NotNull);
            queryExpression.ColumnSet = new ColumnSet("new_name", "new_sobrenome", "new_email", "new_telefone", "new_celular", "new_produtodesejado", "new_idade", "new_numeroderesidencia", "new_bairro", "new_cidade", "new_estado");
            EntityCollection colecaoEntidades = serviceProxyCliente.RetrieveMultiple(queryExpression);

            foreach (var entidade in colecaoEntidades.Entities)
            {

                try
                {
                    var lead = new Entity("tcc_clientepotencial");
                    Guid registroConta = new Guid();

                    if (entidade.Attributes.Contains("new_leadid"))
                        lead.Attributes.Add("tcc_clientepotencialid", entidade.Id);

                    if (entidade.Attributes.Contains("new_name"))
                        lead.Attributes.Add("tcc_nome", entidade.GetAttributeValue<string>("new_name"));

                    if (entidade.Attributes.Contains("new_sobrenome"))
                        lead.Attributes.Add("tcc_sobrenome", entidade.GetAttributeValue<string>("new_sobrenome"));

                    if (entidade.Attributes.Contains("new_email"))
                        lead.Attributes.Add("tcc_email", entidade.GetAttributeValue<string>("new_email"));

                    if (entidade.Attributes.Contains("new_telefone"))
                        lead.Attributes.Add("tcc_telefone", entidade.GetAttributeValue<string>("new_telefone"));

                    if (entidade.Attributes.Contains("new_leadid"))
                        lead.Attributes.Add("tcc_celular", entidade.GetAttributeValue<string>("new_celular"));

                    if (entidade.Attributes.Contains("new_produtodesejado"))
                        lead.Attributes.Add("tcc_produtos", entidade.GetAttributeValue<EntityReference>("new_produtodesejado"));

                    if (entidade.Attributes.Contains("new_idade"))
                        lead.Attributes.Add("tcc_idade", entidade.GetAttributeValue<int>("new_idade").ToString());

                    if (entidade.Attributes.Contains("new_numeroderesidencia"))
                        lead.Attributes.Add("tcc_endereco", entidade.GetAttributeValue<string>("new_numeroderesidencia"));

                    if (entidade.Attributes.Contains("new_bairro"))
                        lead.Attributes.Add("tcc_bairro", entidade.GetAttributeValue<string>("new_bairro"));

                    if (entidade.Attributes.Contains("new_cidade"))
                        lead.Attributes.Add("tcc_cidade", entidade.GetAttributeValue<string>("new_cidade"));

                    if (entidade.Attributes.Contains("new_estado"))
                        lead.Attributes.Add("tcc_estado", entidade.GetAttributeValue<string>("new_estado"));

                    registroConta = serviceProxy.Create(lead);
                }

                catch (Exception e)
                {

                    Console.WriteLine("{0} Exception caught.", e);

                }




            }

            return colecaoEntidades;

        }

        static EntityCollection RetornarMultiploClienteEuRoboListadeitens(CrmServiceClient serviceProxy, CrmServiceClient serviceProxyCliente)
        {
            QueryExpression queryExpression = new QueryExpression("new_listadeitens");

            queryExpression.Criteria.AddCondition("new_item", ConditionOperator.NotNull);
            queryExpression.Criteria.AddCondition("new_quantidade", ConditionOperator.NotNull);
            queryExpression.Criteria.AddCondition("new_numeroitemtriologia", ConditionOperator.NotNull);
            queryExpression.ColumnSet = new ColumnSet("new_item", "new_quantidade", "new_numeroitemtriologia");
            EntityCollection colecaoEntidades = serviceProxyCliente.RetrieveMultiple(queryExpression);

            foreach (var entidade in colecaoEntidades.Entities)
            {

                try
                {
                    var lista = new Entity("tcc_listadeitens");
                    Guid registroLista = new Guid();

                    if (entidade.Attributes.Contains("new_listadeitensid"))
                        lista.Attributes.Add("tcc_listadeitensid", entidade.Id);

                    if (entidade.Attributes.Contains("new_item"))
                        lista.Attributes.Add("tcc_itens", entidade.GetAttributeValue<EntityReference>("new_item"));

                    if (entidade.Attributes.Contains("new_quantidade"))
                        lista.Attributes.Add("tcc_quantidade", entidade.GetAttributeValue<int>("new_quantidade"));

                    if (entidade.Attributes.Contains("new_numeroitemtriologia"))
                        lista.Attributes.Add("tcc_numeropedidoitem", entidade.GetAttributeValue<EntityReference>("new_numeroitemtriologia"));

                    registroLista = serviceProxy.Create(lista);
                }

                catch (Exception e)
                {

                    Console.WriteLine("{0} Exception caught.", e);

                }




            }

            return colecaoEntidades;

        }
        #endregion
    }
}