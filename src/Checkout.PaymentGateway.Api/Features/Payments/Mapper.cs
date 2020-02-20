using AutoMapper;
using Checkout.PaymentGateway.Domain.Payments;

namespace Checkout.PaymentGateway.Api.Features.Payments
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Payment, Get.Model>()
                .ForMember(x => x.PaymentId, src => src.MapFrom(x => x.Id))
                .ForMember(x => x.Status, src => src.MapFrom(x => x.Status.Value));

            CreateMap<Card, Get.Model.CardModel>()
                .ForMember(x => x.CardNumber, src => src.MapFrom(x => x.Number.Mask(3)))
                .ForMember(x => x.ExpiryMonth, src => src.MapFrom(x => x.ExpiryDate.Month))
                .ForMember(x => x.ExpiryYear, src => src.MapFrom(x => x.ExpiryDate.Year));

            CreateMap<Price, Get.Model.PriceModel>();
        }
    }
}
