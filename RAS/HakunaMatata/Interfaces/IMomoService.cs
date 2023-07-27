using System.Threading.Tasks;
using HakunaMatata.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IMomoService
    {
        Task<ResponseUriModel> CreatePayment(PaymentInfoModel dto);

        PaymentResponseModel PaymentExecute(IQueryCollection collection);
    }
}