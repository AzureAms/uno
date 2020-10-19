﻿#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tizen.Applications;
using Tizen.Pims.Contacts;
using Windows.ApplicationModel.Contacts;
using Uno.UI.Runtime.Skia.Tizen.Helpers;
using System;
using TizenContact = Tizen.Pims.Contacts.ContactsViews.Contact;
using TizenEmail = Tizen.Pims.Contacts.ContactsViews.Email;
using TizenName = Tizen.Pims.Contacts.ContactsViews.Name;
using TizenNumber = Tizen.Pims.Contacts.ContactsViews.Number;
using TizenAddress = Tizen.Pims.Contacts.ContactsViews.Address;

namespace Uno.UI.Runtime.Skia.Tizen.ApplicationModel.Contacts
{
	public class ContactPickerExtension : IContactPickerExtension
	{
		private const string ContactMimeType = "application/vnd.tizen.contact";

		public Task<bool> IsSupportedAsync() => Task.FromResult(true);

		public async Task<Contact?> PickContactAsync()
		{
			var results = await PickContactsAsync(false);
			return results.FirstOrDefault();
		}

		private async Task<Contact[]> PickContactsAsync(bool pickMultiple)
		{
			if (!await PrivilegesHelper.RequestAsync(Privileges.ContactsRead))
			{
				return Array.Empty<Contact>();
			}

			var completionSource = new TaskCompletionSource<Contact[]>();

			var appControl = new AppControl
			{
				Operation = AppControlOperations.Pick,
				LaunchMode = AppControlLaunchMode.Single,
				Mime = ContactMimeType
			};
			appControl.ExtraData.Add(AppControlData.SectionMode, pickMultiple ? "multiple" : "single");

			AppControl.SendLaunchRequest(appControl, (request, reply, pickResult) =>
			{
				var results = new List<Contact>();

				if (pickResult == AppControlReplyResult.Succeeded)
				{
					var contactsManager = new ContactsManager();

					var pickedContacts = reply.ExtraData.Get<IEnumerable<string>>(AppControlData.Selected);

					foreach (var pickedContactId in pickedContacts)
					{
						if (int.TryParse(pickedContactId, out var contactId))
						{
							var tizenContact = contactsManager.Database.Get(TizenContact.Uri, contactId);
							if (tizenContact != null)
							{
								var contact = ContactFromContactsRecord(tizenContact);
								results.Add(contact);
							}
						}
					}
				}

				completionSource.TrySetResult(results.ToArray());
			});

			return await completionSource.Task;
		}

		private static Contact ContactFromContactsRecord(ContactsRecord contactsRecord)
		{
			var contact = new Contact();

			var recordName = contactsRecord.GetChildRecord(TizenContact.Name, 0);
			if (recordName != null)
			{
				contact.HonorificNamePrefix = recordName.Get<string>(TizenName.Prefix) ?? string.Empty;
				contact.FirstName = recordName.Get<string>(TizenName.First) ?? string.Empty;
				contact.MiddleName = recordName.Get<string>(TizenName.Addition) ?? string.Empty;
				contact.LastName = recordName.Get<string>(TizenName.Last) ?? string.Empty;
				contact.HonorificNameSuffix = recordName.Get<string>(TizenName.Suffix) ?? string.Empty;

				contact.YomiGivenName = recordName.Get<string>(TizenName.PhoneticFirst) ?? string.Empty;
				contact.YomiFamilyName = recordName.Get<string>(TizenName.PhoneticLast) ?? string.Empty;			
			}

			var emailCount = contactsRecord.GetChildRecordCount(TizenContact.Email);
			for (var mailId = 0; mailId < emailCount; mailId++)
			{
				var emailRecord = contactsRecord.GetChildRecord(TizenContact.Email, mailId);
				var address = emailRecord.Get<string>(TizenEmail.Address);
				var type = (TizenEmail.Types)emailRecord.Get<int>(TizenEmail.Type);

				contact.Emails.Add(new ContactEmail()
				{
					Address = address,
					Kind = GetContactEmailKind(type)
				});
			}

			var phoneCount = contactsRecord.GetChildRecordCount(TizenContact.Number);
			for (var phoneId = 0; phoneId < phoneCount; phoneId++)
			{
				var phoneRecord = contactsRecord.GetChildRecord(TizenContact.Number, phoneId);
				var number = phoneRecord.Get<string>(TizenNumber.NumberData);
				var type = (TizenNumber.Types)phoneRecord.Get<int>(TizenNumber.Type);

				contact.Phones.Add(new ContactPhone()
				{
					Number = number,
					Kind = GetContactPhoneKind(type)
				});
			}

			var addressCount = contactsRecord.GetChildRecordCount(TizenContact.Address);
			for (var addressId = 0; addressId < addressCount; addressId++)
			{
				var addressRecord = contactsRecord.GetChildRecord(TizenContact.Address, addressId);
				var country = addressRecord.Get<string>(TizenAddress.Country);
				var locality = addressRecord.Get<string>(TizenAddress.Locality);
				var street = addressRecord.Get<string>(TizenAddress.Street);
				var region = addressRecord.Get<string>(TizenAddress.Region);
				var postalCode = addressRecord.Get<string>(TizenAddress.PostalCode);

				var type = (TizenAddress.Types)addressRecord.Get<int>(TizenAddress.Type);

				contact.Addresses.Add(new ContactAddress()
				{
					Country = country ?? string.Empty,
					Locality = locality ?? string.Empty,
					PostalCode = postalCode ?? string.Empty,
					Region = region ?? string.Empty,
					StreetAddress = street ?? string.Empty,
					Kind = GetContactAddressKind(type)
				});
			}

			return contact;
		}

		private static ContactEmailKind GetContactEmailKind(TizenEmail.Types emailType)
			=> emailType switch
			{
				TizenEmail.Types.Home => ContactEmailKind.Personal,
				TizenEmail.Types.Mobile => ContactEmailKind.Personal,
				TizenEmail.Types.Work => ContactEmailKind.Work,
				_ => ContactEmailKind.Other
			};

		private static ContactPhoneKind GetContactPhoneKind(TizenNumber.Types numberType)
			=> numberType switch
			{
				TizenNumber.Types.Cell => ContactPhoneKind.Mobile,
				TizenNumber.Types.Main => ContactPhoneKind.Mobile,
				TizenNumber.Types.Home => ContactPhoneKind.Home,
				TizenNumber.Types.Pager => ContactPhoneKind.Pager,
				TizenNumber.Types.Assistant => ContactPhoneKind.Assistant,
				TizenNumber.Types.Company => ContactPhoneKind.Company,
				TizenNumber.Types.Fax => ContactPhoneKind.BusinessFax,
				TizenNumber.Types.Radio => ContactPhoneKind.Radio,
				_ => ContactPhoneKind.Other
			};

		private static ContactAddressKind GetContactAddressKind(TizenAddress.Types addressType) =>
			addressType switch
			{
				TizenAddress.Types.Home => ContactAddressKind.Home,
				TizenAddress.Types.Work => ContactAddressKind.Work,
				_ => ContactAddressKind.Other
			};
	}
}
