using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using LabProjectsPortal.Models.Metadata;

namespace LabProjectsPortal.Models
{
    [ModelMetadataType(typeof(UserMetadata))]
    public partial class ApplicationUser
    {
        [DisplayName("Full name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    public partial class Message {
        [DisplayName("Sent")]
        public string SentAtDifference
        {
            get
            {
                var timeDifference = DateTimeOffset.Now - SentAt;

                double seconds = timeDifference.Seconds;
                double minutes = timeDifference.TotalMinutes;
                double hours = timeDifference.TotalHours;
                double days = timeDifference.Days;

                // Construct the formatted string
                string time;
                if (minutes < 1 && hours < 1 && days < 1) 
                    time = string.Format("{0} seconds ago", Math.Floor(seconds));
                else if (minutes > 1 && hours < 1 && days < 1)
                    time = string.Format("{0} minutes ago", Math.Floor(minutes));
                else if (hours <= 12 && days < 1)
                    time = string.Format("{0} hours ago", Math.Floor(hours)); 
                else if(days < 1)
                    time = string.Format("{0}:{1}", SentAt.Hour, SentAt.Minute);
                else
                    time = string.Format("{0}", SentAt.Date.ToShortDateString());

                return time;
            }
        }
    }
}
