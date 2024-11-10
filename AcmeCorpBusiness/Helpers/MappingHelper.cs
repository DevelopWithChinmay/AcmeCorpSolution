using AcmeCorpBusiness.Domain.Contacts;
using AcmeCorpBusiness.Domain.Customers;
using AcmeCorpBusiness.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcmeCorpBusiness.Helpers
{
    public static class MappingHelper
    {
        public static void Configure()
        {
            TypeAdapterConfig<Customer, CustomerFacade>.NewConfig()
                .Map(dest => dest.ContactDetail, src => new ContactDetailFacade
                {
                    Email = src.Email,
                    PhoneNumber = src.PhoneNumber,
                    MailingAddress = src.MailingAddress
                });

            TypeAdapterConfig<CustomerFacade, Customer>.NewConfig()
                .Map(dest => dest.Email, src => src.ContactDetail.Email)
                .Map(dest => dest.PhoneNumber, src => src.ContactDetail.PhoneNumber)
                .Map(dest => dest.MailingAddress, src => src.ContactDetail.MailingAddress);
        }
    }
}
