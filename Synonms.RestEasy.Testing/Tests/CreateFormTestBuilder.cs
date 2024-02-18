using System.Net;
using System.Text.Json;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Schema;
using Synonms.RestEasy.Core.Schema.Forms;
using Synonms.RestEasy.Testing.Extensions;
using Synonms.RestEasy.Testing.Assertions;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public static class CreateFormTest
{
    public static CreateFormTestBuilder Create(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, Action<Form> validateFormAction) => 
        new(httpClient, collectionPath, jsonSerialiserOptions, validateFormAction);
}

public class CreateFormTestBuilder
{
    public CreateFormTestBuilder(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, Action<Form> validateFormAction)
    {
        Arrange = new ArrangeStep(httpClient, collectionPath, jsonSerialiserOptions, validateFormAction);
    }
    
    public ArrangeStep Arrange { get; }
    
    public class ArrangeStep
    {
        public ArrangeStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, Action<Form> validateFormAction)
        {
            Act = new ActStep(httpClient, collectionPath, jsonSerialiserOptions, validateFormAction);
        }
        
        public ActStep Act { get; }
    }
    
    public class ActStep
    {
        private readonly HttpClient _httpClient;

        public ActStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, Action<Form> validateFormAction)
        {
            _httpClient = httpClient;
            Assert = new AssertStep(httpClient, collectionPath, jsonSerialiserOptions, validateFormAction);
        }

        public AssertStep Assert { get; }
        
        public ActStep WithApiVersion(int apiVersion)
        {
            _httpClient.WithApiVersion(apiVersion);

            return this;
        }

        public ActStep WithAuthenticatedUser(string userId, params string[] permissions)
        {
            _httpClient.WithAuthenticatedUser(userId, permissions);
            
            return this;
        }

        // public ActStep WithEntityTag(EntityVersion entityVersion)
        // {
        //     _httpClient.WithEntityTag(entityVersion);
        //     
        //     return this;
        // }
        
        public ActStep WithCorrelationId(Guid correlationId)
        {
            _httpClient.WithCorrelationId(correlationId);

            return this;
        }
    }
    
    public class AssertStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly string _createFormPath;
        private readonly Action<Form> _validateFormAction;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;

        public AssertStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, Action<Form> validateFormAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _validateFormAction = validateFormAction;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            
            _createFormPath = collectionPath + "/" + IanaLinkRelations.Forms.Create;
        }

        public void SucceedsWith(HttpStatusCode httpStatusCode)
        {
            HttpResponseMessage httpResponseMessage = _httpClient.GetAsync(_createFormPath).Result;

            httpResponseMessage.AssertSuccess(httpStatusCode);

            string content = httpResponseMessage.Content.ReadAsStringAsync().Result;

            FormDocument? formDocument = JsonSerializer.Deserialize<FormDocument>(content, _jsonSerialiserOptions);

            if (formDocument is null)
            {
                Assert.Fail($"Failed to deserialise {nameof(FormDocument)} from response: {content}");
                return;
            }

            Assert.Equal(TestUriConverter.ToAbsoluteUri(_collectionPath), formDocument.Form.Target.Uri);
            Assert.Equal(IanaLinkRelations.Forms.Create, formDocument.Form.Target.Relation);
            Assert.Equal(IanaHttpMethods.Post, formDocument.Form.Target.Method);
            
            _validateFormAction.Invoke(formDocument.Form);
            
            Dictionary<string, Link> expectedLinks = new()
            {
                [IanaLinkRelations.Self] = Link.SelfLink(TestUriConverter.ToAbsoluteUri(_createFormPath))
            };
            
            AssertThat.Links(formDocument.Links).Presents(expectedLinks);
        }
        
        public void FailsWith(HttpStatusCode httpStatusCode)
        {
            HttpResponseMessage httpResponseMessage = _httpClient.GetAsync(_createFormPath).Result;

            httpResponseMessage.AssertFailure(httpStatusCode);
        }
    }
}