﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMSApi.Api;
using SMSApi.Api.Response;

namespace smsapiTests.Contacts
{
    [TestClass]
    public class EditContactTest : ContactsTestBase
    {
        private Contact _contact;

        [TestInitialize]
        [TestCleanup]
        public void Cleanup()
        {
            var contactsResponse = _factory.ListContacts().SetPhoneNumber(_validTestNumber).Execute();
            if (contactsResponse.Collection.Count > 0)
            {
                _contact = contactsResponse.Collection[0];
            }
            else
            {
                _contact = _factory.CreateContact().SetPhoneNumber(_validTestNumber).Execute();
            }
        }

        [TestMethod]
        public void TestEditContact()
        {
            var editContact = _factory.EditContact(_contact.Id)
                                .SetFirstName("Tester")
                                .Execute();

            Assert.AreEqual(_contact.Id, editContact.Id);
            Assert.AreEqual("Tester", editContact.FirstName);
        }
    }
}
