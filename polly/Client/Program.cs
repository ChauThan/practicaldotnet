using Polly;

var retryPolicy = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
    .RetryAsync(3, onRetry: (result, attempt) =>
    {
        var response = result.Result;
        Console.WriteLine($"Attempt: {attempt} | Status code: {(int)response.StatusCode}");
    });


var httpClient = new HttpClient();
var response = await retryPolicy.ExecuteAsync(
    () => httpClient.GetAsync("https://localhost:7279/getdata/20")
);

if (response.IsSuccessStatusCode)
{
    Console.WriteLine($"Status code: {response.StatusCode}");
}