using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using LynkApi;
using System.Net;
using Newtonsoft.Json;
using TestLynk;
using static LynkApi.Appointment;

namespace LynkApi
{
    public class ApiClient
    {
        public ApiClient(Uri baseAddress, string apiToken) //konstruktor
        {
            if (string.IsNullOrEmpty(apiToken))
            {
                throw new ArgumentException($"'{nameof(apiToken)}' cannot be null or empty.", nameof(apiToken));
            }

            this.BaseAddress = baseAddress ?? throw new ArgumentNullException(nameof(baseAddress));
            
            this.Client = new HttpClient();
            this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);  //authentication header
        }

        public HttpClient Client { get; } //properties

        public Uri BaseAddress { get; }

        
        public async Task<IEnumerable<Workshop>?> GetWorkshops() //async makes sure that our website doesnt lock up. IEnumerable supports a simple iteration over a (non-generic) collection.
        {
            //variabel workshopUri
            var workshopUri = new Uri(BaseAddress, "locations/"); //puts together baseadress and endpoint
            
            using (HttpResponseMessage response = await Client.GetAsync(workshopUri)) //new call/request from our api client and wait for response
            {
                if (response.IsSuccessStatusCode) //If successfull do something with it (read the data that came back)
                {
                    string json = await response.Content.ReadAsStringAsync(); //awaits the response and content is the content of my request
                    var myWorkshops = JsonConvert.DeserializeObject<WorkshopJSON>(json); //deserialize 
                    return myWorkshops?.Workshops;
                   
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }

        }

        public async Task<IEnumerable<Appointment>?> GetAppointments(int workshopId)
        {
            var appointmentUri = new Uri(BaseAddress, "appointments/");
            using (HttpResponseMessage response = await Client.GetAsync(appointmentUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var myAppointments = JsonConvert.DeserializeObject<AppointmentJSON>(json);
                    return myAppointments?.Appointments;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }

        }

        

    }
}
