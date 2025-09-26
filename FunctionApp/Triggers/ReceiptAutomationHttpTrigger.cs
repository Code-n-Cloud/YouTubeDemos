using Azure;
using Azure.AI.DocumentIntelligence;
using ClassLibrary.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FunctionApp.Triggers;

public class ReceiptAutomationHttpTrigger(ILogger<ReceiptAutomationHttpTrigger> _logger, IConfiguration configuration)
{
    [Function("ReceiptAutomationHttpTrigger")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        string base64String = await new StreamReader(req.Body).ReadToEndAsync();

        var receipt = await GetReceiptContent(base64String);
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");

        var options = new System.Text.Json.JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(receipt, options);
        await response.WriteStringAsync(jsonResponse);
        return response;
    }
    private async Task<Receipt> GetReceiptContent(string base64String)
    {
        Receipt receipt = new Receipt();
        try
        {
            string endpoint = configuration["AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT"];
            string apiKey = configuration["AZURE_DOCUMENT_INTELLIGENCE_KEY"];
            AzureKeyCredential credential = new AzureKeyCredential(apiKey);
            DocumentIntelligenceClient client = new DocumentIntelligenceClient(new Uri(endpoint), credential);
            BinaryData documentData = BinaryData.FromBytes(Convert.FromBase64String(base64String));
            Operation<AnalyzeResult> operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-receipt", documentData);

            AnalyzeResult receipts = operation.Value;


            AnalyzedDocument receiptDocument = receipts.Documents.FirstOrDefault();
            if (receiptDocument.Fields.TryGetValue("CountryRegion", out DocumentField fieldCountryRegion))
            {
                receipt.CountryRegion = fieldCountryRegion.ValueCountryRegion;
            }
            if (receiptDocument.Fields.TryGetValue("Items", out DocumentField fieldItems))
            {
                receipt.Items = new List<ReceiptItem>();
                foreach (var item in fieldItems.ValueList)
                {
                    ReceiptItem receiptItem = new ReceiptItem();
                    foreach (var valueDictionaryItem in item.ValueDictionary)
                    {
                        if (valueDictionaryItem.Key == "Description")
                        {
                            receiptItem.Description = valueDictionaryItem.Value.ValueString;
                        }
                        else if (valueDictionaryItem.Key == "Quantity")
                        {
                            receiptItem.Quantity = Convert.ToInt32(valueDictionaryItem.Value.ValueDouble.Value);
                        }
                        else if (valueDictionaryItem.Key == "TotalPrice")
                        {
                            receiptItem.Price = valueDictionaryItem.Value.ValueCurrency;
                        }
                    }
                    receipt.Items.Add(receiptItem);
                }
            }
            if (receiptDocument.Fields.TryGetValue("MerchantAddress", out DocumentField fieldMerchantAddress))
            {
                receipt.MerchantAddress = fieldMerchantAddress.ValueAddress;
            }
            if (receiptDocument.Fields.TryGetValue("MerchantName", out DocumentField fieldMerchantName))
            {
                receipt.MerchantName = fieldMerchantName.ValueString;
            }
            if (receiptDocument.Fields.TryGetValue("ReceiptType", out DocumentField fieldReceiptType))
            {
                receipt.ReceiptType = fieldReceiptType.ValueString;
            }
            if (receiptDocument.Fields.TryGetValue("Subtotal", out DocumentField fieldSubTotal))
            {
                receipt.SubTotal = fieldSubTotal.ValueCurrency;
            }
            if (receiptDocument.Fields.TryGetValue("Tip", out DocumentField fieldTip))
            {
                receipt.Tip = fieldTip.ValueCurrency;
            }
            if (receiptDocument.Fields.TryGetValue("Total", out DocumentField fieldTotal))
            {
                receipt.Total = fieldTotal.ValueCurrency;
            }
            if (receiptDocument.Fields.TryGetValue("TotalTax", out DocumentField fieldTotalTax))
            {
                receipt.TotalTax = fieldTotalTax.ValueCurrency;
            }
            if (receiptDocument.Fields.TryGetValue("TransactionDate", out DocumentField fieldTransactionDate))
            {
                receipt.TransactionDate = fieldTransactionDate.ValueDate.Value;
            }
            if (receiptDocument.Fields.TryGetValue("TransactionTime", out DocumentField fieldTransactionTime))
            {
                receipt.TransactionTime = fieldTransactionTime.Content;
            }
            if (receiptDocument.Fields.TryGetValue("MerchantPhoneNumber", out DocumentField fieldMerchantPhoneNumber))
            {
                receipt.MerchantPhoneNumber = fieldMerchantPhoneNumber.ValuePhoneNumber;
            }
        }
        catch (Exception)
        {
            throw;
        }
        return receipt;
    }
}