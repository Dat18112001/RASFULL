using HakunaMatata.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IVNPayService
    {
        ResponseUriModel CreatePayment(PaymentInfoModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collection);
    }
}