using System;
using System.Linq;
using Domain.Libs;
using Microsoft.AspNetCore.Http;

namespace Domain.Helpers
{
    public static class PaymentHelper
{
    public static PaymentResponse GetParamPaymentCallBack(IQueryCollection collection, string hashSecret = null)
    {
        var billId = "";
        var transactionId = ""; // paymentId
        var orderId = "";
        var paymentMethod = "";
        var success = true;
        var token = "";
        var payerId = "";
        var customer = "";
        var totalAmount = "";
        
        foreach (var (key, value) in collection)
        {
            if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("bill_id"))
            {
                billId = value;
            }

            if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("transaction_id"))
            {
                transactionId = value;
            }

            if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("order_id"))
            {
                orderId = value;
            }

            if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("payment_method"))
            {
                paymentMethod = value;
            }

            if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("success"))
            {
                success = Convert.ToInt32(value) > 0;
            }

            if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("paymentid"))
            {
                transactionId = value;
            }

            if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("token"))
            {
                token = value;
            }

            if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("payerid"))
            {
                payerId = value;
            }

            if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("customer"))
            {
                customer = value;
            }
            
            if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("total_amount"))
            {
                totalAmount = value;
            }
        }

        if (!paymentMethod.ToLower().Equals("vnpay"))
            return new PaymentResponse()
            {
                Success = success,
                PaymentMethod = "PayPal",
                BillId = billId,
                OrderId = orderId,
                PayerId = payerId,
                PaymentId = transactionId,
                TransactionId = transactionId,
                Token = token,
                Customer = customer,
                TotalAmount = totalAmount
            };

        var vnPayCheck = GetVnPayCallBack(collection, hashSecret);

        return new PaymentResponse()
        {
            Success = true,
            PaymentMethod = "VnPay",
            BillId = billId,
            OrderId = vnPayCheck.OrderId,
            PaymentId = vnPayCheck.TransactionId,
            TransactionId = vnPayCheck.TransactionId,
            Token = vnPayCheck.Token,
            Customer = customer,
            TotalAmount = totalAmount
        };
    }

    private static PaymentResponse GetVnPayCallBack(IQueryCollection collection, string hashSecret)
    {
        var vnPay = new VnPayLibrary();

        foreach (var (key, value) in collection)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                vnPay.AddResponseData(key, value);
            }
        }

        var orderId = Convert.ToInt64(vnPay.GetResponseData("vnp_TxnRef"));
        var vnPayTranId = Convert.ToInt64(vnPay.GetResponseData("vnp_TransactionNo"));
        var vnpResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
        var vnpSecureHash =
            collection.FirstOrDefault(k => k.Key == "vnp_SecureHash").Value; //hash của dữ liệu trả về
        var orderInfo = vnPay.GetResponseData("vnp_OrderInfo");

        var checkSignature =
            vnPay.ValidateSignature(vnpSecureHash, hashSecret); //check Signature

        if (!checkSignature)
            return new PaymentResponse()
            {
                Success = false
            };

        return new PaymentResponse()
        {
            Success = true,
            PaymentMethod = "VnPay",
            BillId = orderInfo,
            OrderId = orderId.ToString(),
            PaymentId = vnPayTranId.ToString(),
            TransactionId = vnPayTranId.ToString(),
            Token = vnpSecureHash
        };
    }
}

public class PaymentResponse
{
    public string OwnerId { get; set; }
    public string UserId { get; set; }
    public string BillId { get; set; }
    public string TransactionId { get; set; }
    public string OrderId { get; set; }
    public string PaymentMethod { get; set; }
    public string PayerId { get; set; }
    public string PaymentId { get; set; }
    public bool Success { get; set; }
    public string Token { get; set; }
    public string Customer { get; set; }
    public string TotalAmount { get; set; }
}
}