using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Options;
using Application.Interfaces;
using HakunaMatata.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace Infrastructure.Services
{
    public class MomoService : IMomoService
{
    private readonly IOptions<MoMoOption> _options;
    private readonly string _paramUrlCallBack = "?payment_method=MoMo&payment_code={0}&success=1&order_id={1}&total_amount={2}";
    private readonly IConfiguration _configuration;

    public MomoService(IOptions<MoMoOption> options, IConfiguration configuration)
    {
        _options = options;
        _configuration = configuration;
    }

    public async Task<ResponseUriModel> CreatePayment(PaymentInfoModel dto)
    {
        var urlCallBack = _configuration["PaymentConfig:ReturnUrl"];
        var orderId = DateTime.UtcNow.Ticks.ToString();
        var returnUrl = urlCallBack + string.Format(_paramUrlCallBack, dto.PaymentCode, orderId, dto.TotalAmount);
        var orderInfo = "Thanh Toan Mua Hang";
        var rawData =
            $"partnerCode={_options.Value.PartnerCode}&accessKey={_options.Value.AccessKey}&requestId={orderId}&amount={dto.TotalAmount}&orderId={orderId}&orderInfo={orderInfo}&returnUrl={returnUrl}&notifyUrl={_options.Value.NotifyUrl}&extraData=";

        var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

        var client = new RestClient(_options.Value.BaseUrl);
        var request = new RestRequest() { Method = RestSharp.Method.Post };
        request.AddHeader("Content-Type", "application/json; charset=UTF-8");
        
        // Create an object representing the request data
        var requestData = new
        {
            accessKey = _options.Value.AccessKey,
            partnerCode = _options.Value.PartnerCode,
            requestType = _options.Value.RequestType,
            notifyUrl = _options.Value.NotifyUrl,
            returnUrl = returnUrl,
            orderId = orderId,
            amount = dto.TotalAmount.ToString(),
            orderInfo = orderInfo,
            requestId = orderId,
            extraData = "",
            signature = signature
        };

        request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

        var response = await client.ExecuteAsync(request);
        
        var model = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content ?? "{}");

        return new ResponseUriModel()
        {
            Name = "MoMo",
            Uri = model?.PayUrl ?? "/"
        };
    }

    public PaymentResponseModel PaymentExecute(IQueryCollection collection)
    {
        var amount = collection.First(s => s.Key == "amount").Value;
        var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
        var orderId = collection.First(s => s.Key == "orderId").Value;
        var totalAmount = collection.First(s => s.Key == "total_amount").Value;
        
        var res = new MomoExecuteResponseModel()
        {
            Amount = amount,
            OrderId = orderId,
            OrderInfo = orderInfo,
            TotalAmount = totalAmount
        };
        
        return new PaymentResponseModel()
        {
            Success = !string.IsNullOrEmpty(res.OrderId),
            TotalAmount = res.TotalAmount,
            OrderId = res.OrderId,
        };
    }
    
    private string ComputeHmacSha256(string message, string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        byte[] hashBytes;

        using (var hmac = new HMACSHA256(keyBytes))
        {
            hashBytes = hmac.ComputeHash(messageBytes);
        }

        var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return hashString;
    }
}
}