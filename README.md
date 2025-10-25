# BurnerMail.cs
Web-API for [burnermail.io](https://burnermail.io/) an tool to generate a random email address for every form that you're submitting online, that will forward all emails to your personal email address

## Example
```cs
using System;
using BurnerMailApi;

namespace Application
{
    internal class Program
    {
        static async Task Main()
        {
            var api = new BurnerMail();
            string hash = await api.GetHash();
            Console.WriteLine(hash);
        }
    }
}
```
