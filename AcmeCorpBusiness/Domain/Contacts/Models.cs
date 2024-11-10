﻿namespace AcmeCorpBusiness.Domain.Contacts
{
    public record ContactDetailFacade
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MailingAddress { get; set; }
    }
}
