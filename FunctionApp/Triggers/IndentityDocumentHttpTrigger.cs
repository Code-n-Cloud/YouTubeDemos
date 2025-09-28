using Azure;
using Azure.AI.DocumentIntelligence;
using ClassLibrary.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FunctionApp.Triggers;

public class IndentityDocumentHttpTrigger(ILogger<IndentityDocumentHttpTrigger> logger, IConfiguration configuration)
{
    [Function("IndentityDocumentHttpTrigger")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");
        string base64String = await new StreamReader(req.Body).ReadToEndAsync();
        byte[] documentContent = Convert.FromBase64String(base64String);
        IdentityDocuemnt result = await GetIdentityDocumentInformation(documentContent);
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(result, new JsonSerializerOptions() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }));
        return response;
    }
    private async Task<IdentityDocuemnt> GetIdentityDocumentInformation(byte[] documentContent)
    {
        string endpoint = configuration["AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT"];
        string key = configuration["AZURE_DOCUMENT_INTELLIGENCE_KEY"]; ;
        AzureKeyCredential credential = new AzureKeyCredential(key);
        DocumentIntelligenceClient client = new DocumentIntelligenceClient(new Uri(endpoint), credential);
        BinaryData idDocumentUri = BinaryData.FromBytes(documentContent);
        Operation<AnalyzeResult> operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-idDocument", idDocumentUri);

        IdentityDocuemnt identityDocuemnt = new IdentityDocuemnt();
        AnalyzeResult identityDocuments = operation.Value;
        AnalyzedDocument analyzedDocument = identityDocuments.Documents.FirstOrDefault();

        foreach (KeyValuePair<string, DocumentField> field in analyzedDocument.Fields)
        {
            if (field.Key == "Address")
            {
                identityDocuemnt.Address = field.Value.ValueAddress;
            }
            else if (field.Key == "CountryRegion")
            {
                identityDocuemnt.Country = field.Value.ValueCountryRegion;
            }
            else if (field.Key == "DateOfBirth")
            {
                identityDocuemnt.DateOfBirth = field.Value.ValueDate.Value;
            }
            else if (field.Key == "DateOfExpiration")
            {
                identityDocuemnt.DateOfExpiration = field.Value.ValueDate.Value;
            }
            else if (field.Key == "DateOfIssue")
            {
                identityDocuemnt.DateOfIssue = field.Value.ValueDate.Value;
            }
            else if (field.Key == "DocumentDiscriminator")
            {
                identityDocuemnt.DocumentDiscriminator = field.Value.ValueString;
            }
            else if (field.Key == "DocumentNumber")
            {
                identityDocuemnt.DocumentNumber = field.Value.ValueString;
            }
            else if (field.Key == "Endorsements")
            {
                identityDocuemnt.Endorsements = field.Value.ValueString;
            }
            else if (field.Key == "EyeColor")
            {
                identityDocuemnt.EyeColor = field.Value.ValueString;
            }
            else if (field.Key == "FirstName")
            {
                identityDocuemnt.FirstName = field.Value.ValueString;
            }
            else if (field.Key == "LastName")
            {
                identityDocuemnt.LastName = field.Value.ValueString;
            }
            else if (field.Key == "Height")
            {
                identityDocuemnt.Height = field.Value.ValueString;
            }
            else if (field.Key == "Region")
            {
                identityDocuemnt.Region = field.Value.ValueString;
            }
            else if (field.Key == "Restrictions")
            {
                identityDocuemnt.Restrictions = field.Value.ValueString;
            }
            else if (field.Key == "Sex")
            {
                identityDocuemnt.Sex = field.Value.ValueString;
            }
            else if (field.Key == "VehicleClassifications")
            {
                identityDocuemnt.VehicleClassifications = field.Value.ValueString;
            }
            else if (field.Key == "Weight")
            {
                identityDocuemnt.Weight = field.Value.ValueString;
            }
            else if (field.Key == "PersonalNumber")
            {
                identityDocuemnt.PersonalNumber = field.Value.ValueString;
            }
            else if (field.Key == "PlaceOfBirth")
            {
                identityDocuemnt.PlaceOfBirth = field.Value.ValueString;
            }
            else if (field.Key == "Category")
            {
                identityDocuemnt.Category = field.Value.ValueString;
            }
            else if (field.Key == "DocumentType")
            {
                identityDocuemnt.DocumentType = field.Value.ValueString;
            }
            else if (field.Key == "IssuingAuthority")
            {
                identityDocuemnt.IssuingAuthority = field.Value.ValueString;
            }
            else if (field.Key == "Nationality")
            {
                identityDocuemnt.Nationality = field.Value.ValueString;
            }
            else if (field.Key == "MachineReadableZone")
            {
                MachineReadableZone machineReadableZone = new MachineReadableZone();
                foreach (var item in field.Value.ValueDictionary)
                {
                    if (item.Key == "CountryRegion")
                    {
                        machineReadableZone.Country = item.Value.ValueCountryRegion;
                    }
                    else if (item.Key == "DateOfBirth")
                    {
                        machineReadableZone.DateOfBirth = item.Value.ValueDate.Value;
                    }
                    else if (item.Key == "DateOfExpiration")
                    {
                        machineReadableZone.DateOfExpiration = item.Value.ValueDate.Value;
                    }
                    else if (item.Key == "DocumentNumber")
                    {
                        machineReadableZone.DocumentNumber = item.Value.ValueString;
                    }
                    else if (item.Key == "FirstName")
                    {
                        machineReadableZone.FirstName = item.Value.ValueString;
                    }
                    else if (item.Key == "LastName")
                    {
                        machineReadableZone.LastName = item.Value.ValueString;
                    }
                    else if (item.Key == "Nationality")
                    {
                        machineReadableZone.Nationality = item.Value.ValueString;
                    }
                    else if (item.Key == "Sex")
                    {
                        machineReadableZone.Sex = item.Value.ValueString;
                    }
                }
                identityDocuemnt.MachineReadableZone = machineReadableZone;
            }
            else
            {
            }
        }
        return identityDocuemnt;
    }
}