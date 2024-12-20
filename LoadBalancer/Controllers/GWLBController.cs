using System.Text;
using LoadBalancer.Model;
using LoadBalancer.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LoadBalancer.Controllers;

[ApiController]
[Route("[controller]")]
public class GWLBController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly Instances _instances;

    // ServiceDiscovery sends a Service to the GatewayLoadbalancer
    public GWLBController(HttpClient httpClient, Instances instances)
    {
        _httpClient = httpClient;
        _instances = instances;
    }

    // Updates the instances (available services)
    [HttpPost("updateInstance")]
    public async Task<IActionResult> UpdateInstance([FromBody] List<MicroServiceInstance> incInstance)
    {
        _instances.OrderServiceInstances.Clear();
        _instances.InventoryServiceInstances.Clear();

        foreach (var instance in incInstance)
            if (instance.ServiceName == "OrderService")
                _instances.OrderServiceInstances.Add(instance);
            else if (instance.ServiceName == "InventoryService") _instances.InventoryServiceInstances.Add(instance);

        return await Task.FromResult<IActionResult>(Ok());
    }

    [Route("order")]
    [AcceptVerbs("GET", "POST", "PUT", "DELETE")]
    public async Task<IActionResult> ForwardToOrderService(string action, [FromQuery] int id = 0,
        [FromBody] Order? order = null)
    {
        return await ForwardRequest("OrderService", "Order", action, id, order!);
    }

    [Route("inventory")]
    [AcceptVerbs("GET", "POST", "PUT", "DELETE")]
    public async Task<IActionResult> ForwardToInventoryService(string action, [FromBody] Product? product, [FromQuery] int id = 0)
    {
        System.Console.WriteLine(product.Name);
        return await ForwardRequest("InventoryService", "Inventory", action, id, product!);
    }


    private async Task<IActionResult> ForwardRequest(string serviceName, string controllerName, string action, int id,
        object objectToForward)
    {
        // Round Robin Load Balancing - calls the method that Load Balances the requests
        var instanceUrl = GetNextInstance(serviceName);

        // Forward the request
        var targetUrl = $"http://{instanceUrl.IpAddress}:{instanceUrl.Port}/{controllerName}/{action}";
        if (id > 0) targetUrl += $"?id={id}";

        try
        {
            HttpResponseMessage response;

            // Determine HTTP method from the current request
            StringContent? requestContent = null;
            if (objectToForward != null)
            {
                var json = JsonConvert.SerializeObject(objectToForward);
                requestContent = new StringContent(json, Encoding.UTF8, "application/json");
            }

            switch (HttpContext.Request.Method.ToUpper())
            {
                case "GET":
                    response = await _httpClient.GetAsync(targetUrl);
                    break;
                case "POST":
                    System.Console.WriteLine("gateway controller" + objectToForward.GetType().Name);
                    response = await _httpClient.PostAsync(targetUrl, requestContent);
                    break;
                case "PUT":

                    response = await _httpClient.PutAsync(targetUrl, requestContent);
                    break;
                case "DELETE":
                    response = await _httpClient.DeleteAsync(targetUrl);
                    break;
                default:
                    return StatusCode(405, "Method Not Allowed"); // Return 405 if method not supported
            }

            // Handle response from the target service
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in forwarding request: {e.Message}");
            return StatusCode(500, $"Error forwarding request: {e.Message}");
        }
    }

    private MicroServiceInstance GetNextInstance(string serviceName)
    {
        if (serviceName == "OrderService")
        {
            var index = _instances.RoundRobinIndex.GetValueOrDefault(serviceName, -1);

            index = (index + 1) % _instances.OrderServiceInstances.Count;

            _instances.RoundRobinIndex[serviceName] = index;
            return _instances.OrderServiceInstances[index];
        }
        else
        {
            var index = _instances.RoundRobinIndex.GetValueOrDefault(serviceName, -1);
            index = (index + 1) % _instances.InventoryServiceInstances.Count;

            _instances.RoundRobinIndex[serviceName] = index;
            return _instances.InventoryServiceInstances[index];
        }
    }
}