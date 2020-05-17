using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServerlessMoedas.Data;

namespace ServerlessMoedas
{
    public class CotacoesHttpTrigger
    {
        private MoedasContext _context;

        public CotacoesHttpTrigger(MoedasContext context)
        {
            _context = context;
        }

        [FunctionName("CotacoesHttpTrigger")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string moeda = req.Query["moeda"];
            log.LogInformation($"CotacoesHttpTrigger: {moeda}");

            if (!String.IsNullOrWhiteSpace(moeda))
            {
                return (ActionResult)new OkObjectResult(
                    _context.Cotacoes
                        .Where(c => c.Sigla == moeda)
                        .FirstOrDefault()
                    );
            }
            else
            {
                return new BadRequestObjectResult(new
                {
                    Sucesso = false,
                    Mensagem = "Informe uma sigla de moeda válida"
                });
            }
        }
    }
}