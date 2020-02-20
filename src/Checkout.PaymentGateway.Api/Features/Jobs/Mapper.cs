using AutoMapper;
using Checkout.PaymentGateway.Api.Messaging;

namespace Checkout.PaymentGateway.Api.Features.Jobs
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Job, Get.Model>();
        }
    }
}
