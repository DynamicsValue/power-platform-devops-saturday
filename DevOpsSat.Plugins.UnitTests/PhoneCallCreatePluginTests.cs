using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using TypedEntities;
using Xunit;

namespace DevOpsSat.Plugins.UnitTests
{
    public class PhoneCallCreatePluginTests
    {
        private readonly Contact _contact;
        private readonly PhoneCall _phoneCall;
        private readonly IOrganizationService _service;
        private readonly XrmFakedContext _context;

        public PhoneCallCreatePluginTests()
        {
            _context = new XrmFakedContext();
            _service = _context.GetOrganizationService();

            _contact = new Contact() { Id = Guid.NewGuid() };

            _phoneCall = new PhoneCall()
            {
                Id = Guid.NewGuid(),
                PhoneNumber = "3650000",
                RegardingObjectId = _contact.ToEntityReference()
            };
        }

        [Fact]
        public void Should_create_phonecall_history_record_for_contact_and_phonenumber()
        {
            _context.ExecutePluginWithTarget<PhoneCallCreatePlugin>(_phoneCall);

            var phoneCallHistory = _context.CreateQuery<ultra_phonecallhistory>().FirstOrDefault();
            Assert.NotNull(phoneCallHistory);

            Assert.Equal(_contact.Id, phoneCallHistory.ultra_contactid.Id);
            Assert.Equal(_phoneCall.PhoneNumber, phoneCallHistory.ultra_phonenumber);

        }

        [Fact]
        public void Should_not_create_phonecall_history_record_for_the_same_phonenumber_and_contact_pair()
        {
            var existingPhoneCallHistoryRecord = new ultra_phonecallhistory()
            {
                Id = Guid.NewGuid(),
                ultra_contactid = _contact.ToEntityReference(),
                ultra_phonenumber = _phoneCall.PhoneNumber
            };

            _context.Initialize(existingPhoneCallHistoryRecord);

            _context.ExecutePluginWithTarget<PhoneCallCreatePlugin>(_phoneCall);

            var phoneCallHistoryList = _context.CreateQuery<ultra_phonecallhistory>().ToList();
            Assert.Single(phoneCallHistoryList);
        }
    }
}
