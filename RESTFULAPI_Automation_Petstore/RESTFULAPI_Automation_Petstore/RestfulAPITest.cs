using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

[assembly: Parallelize(Workers = 10, Scope = ExecutionScope.MethodLevel)]
namespace RESTFULAPI_Automation_Petstore
{
    [TestClass]
    public class RestfulAPITest
    {
        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string UserEndpoint = "pet";

        private static string GetURL(string enpoint) => $"{BaseURL}{enpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<UserModel> cleanUpList = new List<UserModel>();
        
        [TestInitialize]
        public async Task TestInitialize()
        {
            restClient = new RestClient();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach (var data in cleanUpList)
            {
                var restRequest = new RestRequest(GetURI($"{UserEndpoint}/{data.Id}"));
                var restResponse = await restClient.DeleteAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task PostMethod()
        {
            #region CreateUser
            //Create User
            var newPet = new UserModel()
            {
                Id = 987,
                Name = "Jappy",
                Status = "Available",
                PhotoUrls = new string[] { "wow" },
                Category = new CategoryModel()
                {
                    Id = 1,
                    Name = "Dog"
                } 
            };

            // Send Post Request
            var temp = GetURI(UserEndpoint);
            var postRestRequest = new RestRequest(GetURI(UserEndpoint)).AddJsonBody(newPet);
            var postRestResponse = await restClient.ExecutePostAsync(postRestRequest);

            //Verify POST request status code
            Assert.AreEqual(HttpStatusCode.OK, postRestResponse.StatusCode, "Status code is not equal to 200");
            #endregion

            #region GetUser
            var restRequest = new RestRequest(GetURI($"{UserEndpoint}/{newPet.Id}"), Method.Get);
            var restResponse = await restClient.ExecuteAsync<UserModel>(restRequest);
            #endregion

            #region Assertions
            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status code is not equal to 200");
            Assert.AreEqual(newPet.Id, restResponse.Data.Id, "Pet ID did not match.");
            Assert.AreEqual(newPet.Name, restResponse.Data.Name, "Pet Name did not match.");
            Assert.AreEqual(newPet.Status, restResponse.Data.Status, "Pet Status did not match.");
            Assert.AreEqual(newPet.Category.Id, restResponse.Data.Category.Id, "Category ID did not match.");
            Assert.AreEqual(newPet.Category.Name, restResponse.Data.Category.Name, "Category Name did not match.");
            Assert.AreEqual(newPet.PhotoUrls[0], restResponse.Data.PhotoUrls[0], "Photo Urls did not match.");
            #endregion

            #region CleanUp
            cleanUpList.Add(newPet);
            #endregion
        }
    }
}
