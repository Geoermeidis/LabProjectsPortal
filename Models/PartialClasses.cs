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
}
