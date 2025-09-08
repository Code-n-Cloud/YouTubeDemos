using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace YoutubeDemos
{
    public class InvoiceAutomationHttpTrigger
    {
        private readonly ILogger<InvoiceAutomationHttpTrigger> logger;
        private readonly IConfiguration configuration;

        public InvoiceAutomationHttpTrigger(ILogger<InvoiceAutomationHttpTrigger> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        [Function("InvoiceAutomation")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            string base64String = await new StreamReader(req.Body).ReadToEndAsync();
            byte[] pdfBytes = Convert.FromBase64String(base64String);


            var invoiceInfo = await GetInvoiceTotal(pdfBytes);


            //string connectionString = Environment.GetEnvironmentVariable("MyStorageConnectionString");
            //string containerName = "invoice-automation"; // Name of your container

            //BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);

            //string blobName = $"pdf-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid()}.pdf";

            //BlobClient blobClient = containerClient.GetBlobClient(blobName);

            //logger.LogInformation($"Uploading to blob: {blobName}");
            //using (MemoryStream stream = new MemoryStream(pdfBytes))
            //{
            //    await blobClient.UploadAsync(stream, overwrite: true);
            //}
            return new OkObjectResult(new { invoiceId = invoiceInfo.Item1, invoiceTotal = invoiceInfo.Item2 });
        }
        private async Task<Tuple<string, double>> GetInvoiceTotal(byte[] pdfBytes)
        {
            string endpoint = configuration["AZURE_OPENAI_ENDPOINT"];
            string key = configuration["AZURE_OPENAI_KEY"];
            string invoiceId = ""; double invoiceTotalAmount = 0;
            try
            {
                AzureKeyCredential credential = new AzureKeyCredential(key);
                DocumentIntelligenceClient client = new DocumentIntelligenceClient(new Uri(endpoint), credential);
                BinaryData documentData = BinaryData.FromBytes(pdfBytes);

                Operation<AnalyzeResult> operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-invoice", documentData);
                AnalyzeResult result = operation.Value;
                for (int i = 0; i < result.Documents.Count; i++)
                {
                    Console.WriteLine($"Document {i}:");

                    AnalyzedDocument document = result.Documents[i];

                    if (document.Fields.TryGetValue("InvoiceTotal", out DocumentField? invoiceTotalField))
                    {
                        if (invoiceTotalField.FieldType == DocumentFieldType.Currency)
                        {
                            CurrencyValue invoiceTotal = invoiceTotalField.ValueCurrency;
                            invoiceTotalAmount = invoiceTotal.Amount;
                        }
                    }
                    if (document.Fields.TryGetValue("InvoiceId", out DocumentField? invoiceIdField))
                    {
                        if (invoiceIdField.FieldType == DocumentFieldType.String)
                        {
                            invoiceId = invoiceIdField.ValueString;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return new Tuple<string, double>(invoiceId, invoiceTotalAmount);
        }
    }
}
