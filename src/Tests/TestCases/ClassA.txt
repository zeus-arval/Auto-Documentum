using System;
using System.Collections.Generic;

namespace NamespaceA
{
	namespace NamespaceB
	{
		public class Person
		{
			private string _idCode;
			private DateTime _birthDay;
			private readonly House? _house;

			private string Address => _house.FullAddress ?? "Homeless";
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public long PhoneNumber { get; set; }

			public Person(string idCode, string address, DateTime birthDay, House? house)
			{
				_idCode = idCode;
				_birthDay = birthDay;
				_address = address;
			}

			public void PrintInfo()
			{
				Console.WriteLine($"{FirstName} {LastName} lives in {_address} and has birthday in {_birthDay}.");
			}

			public string ChangeName(string? firstName, string? lastName)
			{
				FirstName = firstName ?? FirstName;
				LastName = lastName ?? LastName;
			}
		}
	}

	public class House
	{
		public string FullAddress { get; private set; }
		public List<Person?>? Persons { get; private set; }

	}
}