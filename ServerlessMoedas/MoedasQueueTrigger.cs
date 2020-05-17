using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ServerlessMoedas.Data;
using ServerlessMoedas.Models;
using ServerlessMoedas.Validators;

namespace ServerlessMoedas
{
    public class MoedasQueueTrigger
    {
        private MoedasContext _context;

        public MoedasQueueTrigger(MoedasContext context)
        {
            _context = context;
        }

        [FunctionName("MoedasQueueTrigger")]
        public void Run([QueueTrigger("queue-cotacoes", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            bool dadosValidos;
            Cotacao cotacao = null;
            try
            {
                cotacao = JsonSerializer.Deserialize<Cotacao>(myQueueItem);
                var resultadoValidator = new CotacaoValidator().Validate(cotacao);
                dadosValidos = resultadoValidator.IsValid;
                if (!dadosValidos)
                    log.LogError(
                        $"MoedasQueueTrigger - Erros de validação: {resultadoValidator.ToString()}");
            }
            catch
            {
                dadosValidos = false;
                log.LogError($"MoedasQueueTrigger - Erro ao deserializar dados: {myQueueItem}");
            }
            
            if (dadosValidos)
            {
                var dadosCotacao = _context.Cotacoes
                            .Where(c => c.Sigla == cotacao.Sigla)
                            .FirstOrDefault();
                if (dadosCotacao != null)
                {
                    dadosCotacao.UltimaCotacao = DateTime.Now;
                    dadosCotacao.Valor = cotacao.Valor;
                    _context.SaveChanges();
                }

                log.LogInformation($"MoedasQueueTrigger: {myQueueItem}");
            }
        }
    }
}