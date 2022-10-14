﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LynkApi;
using Newtonsoft.Json;
using TestLynk;

namespace LynkApi
{
    public class WorkshopData
    {

        static string dataDirectyory = Path.Combine(Path.GetDirectoryName((typeof(Program).Assembly.Location)) ?? string.Empty, "data"); // skapar filvägen
        static string dataFile = Path.Combine(dataDirectyory, "data.json"); // skapar filen
        public static void GetWorkshopData()
        {
            var baseadress = "https://context-qa.lynkco.com/api/workshop/";
            var apiToken = "y2TpY8nt029M~OC3NdK7tXnpF";

            ApiClient api = new ApiClient(new Uri(baseadress), apiToken);
            var result =  api.GetWorkshops().Result;


            //foreach (var workshop in result) // objekten
            //{
            //    Console.WriteLine("Location Id: " + workshop.LocationId);
            //    Console.WriteLine("Back Office Workshop Id: " + workshop.BackOfficeWorkshopId);
            //    Console.WriteLine("Display Name: " + workshop.DisplayName);
            //    Console.WriteLine("Timezone: " + workshop.TimeZone);
            //}

            var Workshop = JsonConvert.SerializeObject(result); // serialiserar objekten till json
            //Console.WriteLine(Workshop); // skriver ut json

            if (!Directory.Exists(dataDirectyory)) // om directoryn inte finns
            {
                Directory.CreateDirectory(dataDirectyory); // skapa map
                File.WriteAllText(dataFile, Workshop); // skriver ut json till filen
            }
            else
            {
                if (!File.Exists(dataFile)) // om directoryn finns men inte filen
                {
                    File.WriteAllText(dataFile, Workshop);
                }
                else // om både directory och filen finns
                {
                    File.Delete(dataFile);
                    File.WriteAllText(dataFile, Workshop);
                }
            }
            // om man har en data fil gör såhär
            string jsonString = File.ReadAllText(dataFile);

            var objResponse = JsonConvert.DeserializeObject<List<WorkshopModel>>(jsonString); //skriver om JSON till .NET objekt

            // ?? "STR"  <=>  (str == null) ? "STR" : str;
            var WorkshopGroupedByTimeZone = objResponse.GroupBy(objResponse => objResponse.TimeZone ?? "no registerd") // grupperar efter tidszon, om det inte finns så läggs dem tsm i "No Time Zone"
                .OrderByDescending(group => group.Count()); // sorterar desc efter hur många det finns i varje gruppering

            Console.WriteLine("Showing info about " + objResponse.Count + " total workshops:"); //skriver ut totala antl WS
            
            foreach (var group in WorkshopGroupedByTimeZone)
            {
                Console.WriteLine(group.Count() + " workshops has " + group.Key + " Timezone"); //skriver ut tidzonen och hur många WS som finns i varje tidzon
                foreach (var WS in group)
                    Console.WriteLine("* " + " " + WS.DisplayName);
            }
        }
    }
}