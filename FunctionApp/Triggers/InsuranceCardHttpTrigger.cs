using Azure;
using Azure.AI.DocumentIntelligence;
using ClassLibrary.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FunctionApp.Triggers;

public class InsuranceCardHttpTrigger
{
    private readonly ILogger<InsuranceCardHttpTrigger> _logger;
    private readonly IConfiguration _configuration;

    public InsuranceCardHttpTrigger(ILogger<InsuranceCardHttpTrigger> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [Function("InsuranceCardHttpTrigger")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        string base64String = await new StreamReader(req.Body).ReadToEndAsync();
        byte[] insuranceCardBytes = Convert.FromBase64String(base64String);
        InsuranceCardEntity insuranceCardEntity = await GetInsuranceCardDetail(insuranceCardBytes);
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(insuranceCardEntity, new JsonSerializerOptions() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }));
        return response;
    }
    private async Task<InsuranceCardEntity> GetInsuranceCardDetail(byte[] insuranceCardBytes)
    {
        string endPoint = _configuration["AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT"];
        string key = _configuration["AZURE_DOCUMENT_INTELLIGENCE_KEY"];
        InsuranceCardEntity insuranceCardEntity = new InsuranceCardEntity();
        try
        {
            AzureKeyCredential credential = new AzureKeyCredential(key);
            DocumentIntelligenceClient client = new DocumentIntelligenceClient(new Uri(endPoint), credential);
            BinaryData documentData = BinaryData.FromBytes(insuranceCardBytes);
            Operation<AnalyzeResult> operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-healthInsuranceCard.us", documentData);
            AnalyzeResult result = operation.Value;
            AnalyzedDocument document = result.Documents.FirstOrDefault();
            foreach (KeyValuePair<string, DocumentField> field in document.Fields)
            {
                if (field.Key == "IdNumber")
                {
                    insuranceCardEntity.IdNumber = new IdNumber();
                    foreach (var dictionaryItem in field.Value.ValueDictionary)
                    {
                        insuranceCardEntity.IdNumber.Number = dictionaryItem.Value.ValueString;
                    }
                }
                else if (field.Key == "Insurer")
                {
                    insuranceCardEntity.Insurer = field.Value.ValueString;
                }
                else if (field.Key == "MedicareMedicaidInfo")
                {
                    insuranceCardEntity.MedicareMedicaidInfo = new MedicareMedicaidInfo();
                    foreach (var dictionaryItem in field.Value.ValueDictionary)
                    {
                        if (dictionaryItem.Key == "Id")
                        {
                            insuranceCardEntity.MedicareMedicaidInfo.Id = dictionaryItem.Value.ValueString;
                        }
                        else if (dictionaryItem.Key == "PartAEffectiveDate")
                        {
                            insuranceCardEntity.MedicareMedicaidInfo.PartAEffectiveDate = dictionaryItem.Value.ValueDate.Value;
                        }
                        else if (dictionaryItem.Key == "PartBEffectiveDate")
                        {
                            insuranceCardEntity.MedicareMedicaidInfo.PartBEffectiveDate = dictionaryItem.Value.ValueDate.Value;
                        }
                    }
                }
                else if (field.Key == "Member")
                {
                    insuranceCardEntity.Member = new Member();
                    foreach (var dictionaryItem in field.Value.ValueDictionary)
                    {
                        if (dictionaryItem.Key == "Name")
                        {
                            insuranceCardEntity.Member.Name = dictionaryItem.Value.ValueString;
                        }
                    }
                }
                else if (field.Key == "EffectiveDate")
                {
                    insuranceCardEntity.EffectiveDate = field.Value.ValueDate.Value;
                }
                else if (field.Key == "PrescriptionInfo")
                {
                    insuranceCardEntity.PrescriptionInfo = new PrescriptionInfo();
                    foreach (var dictionaryItem in field.Value.ValueDictionary)
                    {
                        if (dictionaryItem.Key == "Issuer")
                        {
                            insuranceCardEntity.PrescriptionInfo.Issuer = dictionaryItem.Value.ValueString;
                        }
                        else if (dictionaryItem.Key == "RxBIN")
                        {
                            insuranceCardEntity.PrescriptionInfo.RxBIN = dictionaryItem.Value.ValueString;
                        }
                        else if (dictionaryItem.Key == "RxGrp")
                        {
                            insuranceCardEntity.PrescriptionInfo.RxGrp = dictionaryItem.Value.ValueString;
                        }
                    }
                }
                else if (field.Key == "Subscriber")
                {
                    insuranceCardEntity.Subscriber = new Subscriber();
                    foreach (var dictionaryItem in field.Value.ValueDictionary)
                    {
                        if (dictionaryItem.Key == "IdNumber")
                        {
                            insuranceCardEntity.Subscriber.IdNumber = dictionaryItem.Value.ValueString;
                        }
                    }
                }
                else if (field.Key == "Plan")
                {
                    insuranceCardEntity.Plan = new Plan();
                    foreach (var dictionaryItem in field.Value.ValueDictionary)
                    {
                        if (dictionaryItem.Key == "Name")
                        {
                            insuranceCardEntity.Plan.Name = dictionaryItem.Value.ValueString;
                        }
                    }
                }
                else
                {
                }
            }
        }
        catch (Exception)
        {
        }
        return insuranceCardEntity;
    }
}