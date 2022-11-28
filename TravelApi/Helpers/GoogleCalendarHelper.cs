using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TravelApi.Calendar;

namespace TravelApi.Helpers
{
    public class GoogleCalendarHelper
    {
        protected GoogleCalendarHelper()
        {

        }
        public static async Task<Event> CreateGoogleCalendar(GoogleCalendar request)
        {
            string[] Scopes = {"https://www.googleapis.com/auth/calendar"};
            string ApplicationName = "Google Calendar API";
            UserCredential credential;
            using (var stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(),"Cre","cre.json"),FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath,true)).Result;
            }
            // define service
            var services = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
            // define request

            Event eventCalendar = new Event() {
                Recurrence = new String[] { "RRULE:FREQ=WEEKLY;BYDAY=MO" },
                Attendees = new List<EventAttendee>()
                {
                    new EventAttendee() { Email = "rotkbynbyn@gmail.com"},
                    new EventAttendee() { Email = "longnhps14949@fpt.edu.vn"},
                    new EventAttendee() { Email = "anhntnps14963@fpt.edu.vn"}

                },
                GuestsCanSeeOtherGuests = true,
                GuestsCanModify = false,
                Summary = request.Summary,
                Location = request.Location,
                Start = new EventDateTime
                {
                    DateTime = request.Start,
                    TimeZone = "Asia/Ho_Chi_Minh"
                },
                End = new EventDateTime
                {
                    DateTime = request.End,
                    TimeZone = "Asia/Ho_Chi_Minh"
                },
                Description = request.Description
                
            };
            var eventRequest = services.Events.Insert(eventCalendar, "primary");
            eventRequest.SendNotifications = true;
            var requestCreate =await eventRequest.ExecuteAsync();
            var b = 123;
            return requestCreate;

        }
    }
}
