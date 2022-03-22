using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using ToDoFunctionApp.Models;

namespace ToDoFunctionApp
{
    public class GetToDoItems
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private MongoClient _client;
        private readonly IMongoCollection<TodoItem> _todolist;
        private string _user;

        public GetToDoItems(ILoggerFactory loggerFactory, IConfiguration config, MongoClient client)
        {
            _logger = loggerFactory.CreateLogger<GetToDoItems>();;
            _config = config;
            _client = client;
            _user = "Lachy"; //context.HttpContext.User.Identity.Name ?? "*";
            var database = _client.GetDatabase(_config[Constants.databaseName]);
            _todolist = database.GetCollection<TodoItem>(_config[Constants.collectionName]);
        }
        
        [OpenApiOperation(operationId: "GetTodoItems")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TodoItem[]), Description = "A to do list")]
        [Function("GetTodoItems")]
        public async Task<HttpResponseData> GetTodoItems(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todos")]
            HttpRequestData req)
        {
            HttpResponseData returnValue = null;

            try
            {
                var result = (await _todolist.FindAsync(item => item.Owner == _user)).ToList();

                if (result == null)
                {
                    _logger.LogInformation($"There are no items in the collection");
                    returnValue = req.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    returnValue = req.CreateResponse(System.Net.HttpStatusCode.OK);
                    await returnValue.WriteAsJsonAsync(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
                returnValue = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return returnValue;
        }
        
        [OpenApiOperation(operationId: "GetTodoItem")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "To do item id")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TodoItem), Description = "A to do item")]
        [Function("GetTodoItem")]
        public async Task<HttpResponseData> GetTodoItem(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", 
                Route = "todos/{id}")]HttpRequestData req, string id)
        {

            try
            {
                var result = (await _todolist.FindAsync(item => item.Id == id && item.Owner == _user)).FirstOrDefault();

                if (result == null)
                {
                    _logger.LogWarning("That item doesn't exist!");
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }
                
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(result);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't find item with id: {id}. Exception thrown: {ex.Message}");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
        
        [OpenApiOperation(operationId: "PostTodoItem")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(TodoItem), Required = true, Description = "To do object that needs to be added to the list")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TodoItem), Description = "A to do item")]
        [Function("PostTodoItem")]
        public async Task<HttpResponseData> PostTodoItem(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todos")] HttpRequestData req)
        {
            HttpResponseData returnValue = null;

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var input = JsonConvert.DeserializeObject<TodoItem>(requestBody);

                var todo = new TodoItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = input.Description,
                    Owner = _user,
                    Status = false
                };

                await _todolist.InsertOneAsync(todo);

                _logger.LogInformation("Todo item inserted");
                returnValue = req.CreateResponse(HttpStatusCode.OK);
                await returnValue.WriteAsJsonAsync(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not insert item. Exception thrown: {ex.Message}");
                returnValue = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return returnValue;
        }

        [OpenApiOperation(operationId: "PutTodoItem")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "To do Id")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(TodoItem), Required = true, Description = "To do object that needs to be updated to the list")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TodoItem), Description = "A to do item")]
        [Function("PutTodoItem")]
        public async Task<HttpResponseData> PutTodoItem(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todos/{id}")] HttpRequestData req,
            string id)
        {
            HttpResponseData returnValue = null;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var updatedResult = JsonConvert.DeserializeObject<TodoItem>(requestBody);

            updatedResult.Id = id;

            try
            {
                var replacedItem = await _todolist.ReplaceOneAsync(item => item.Id == id && item.Owner == _user, updatedResult);

                if (replacedItem == null)
                {
                    returnValue = req.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    returnValue = req.CreateResponse(HttpStatusCode.OK);
                    await returnValue.WriteAsJsonAsync(updatedResult);
                }              
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not update Album with id: {id}. Exception thrown: {ex.Message}");
                returnValue = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return returnValue;
        }

       
        [OpenApiOperation(operationId: "DeleteTodoItem")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "To do Id that needs to be removed from the list")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "OK Response")]
        [Function("DeleteTodoItem")]
        public async Task<HttpResponseData> DeleteTodoItem(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Anonymous, "delete",
            Route = "todos/{id}")]HttpRequestData req, string id)
        {
            HttpResponseData returnValue = null;

            try
            {
                var itemToDelete = await _todolist.DeleteOneAsync(item => item.Id == id && item.Owner == _user);

                if (itemToDelete == null)
                {
                    _logger.LogInformation($"Todo item with id: {id} does not exist. Delete failed");
                    returnValue = req.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                    returnValue = req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not delete item. Exception thrown: {ex.Message}");
                returnValue = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return returnValue;
        }

        [OpenApiOperation(operationId: "HealthCheck")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Status code 200")]
        [Function("HealthCheck")]
        public async Task<HttpResponseData> HealthCheck(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Anonymous, "head",
            Route = "todos")]HttpRequestData req)
        {

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
