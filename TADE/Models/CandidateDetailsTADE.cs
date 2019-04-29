
namespace TADE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    [MetadataType(typeof(CandidateDetailMetadata))]
    public partial class CandidateDetail
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public class CandidateDetailMetadata
        {
            [Required(ErrorMessage = "Email Required!")]
            [DataType(DataType.EmailAddress)]
            [EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password Required!")]
            public string Password { get; set; }

            [Required(ErrorMessage = "First name Required!")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last name Required!")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "select day from dropdown!")]
            public int Day { get; set; }

            [Required(ErrorMessage = "select month from dropdown!")]
            public int Month { get; set; }

            [Required(ErrorMessage = "select year from dropdown!")]
            public int Year { get; set; }

            [Required(ErrorMessage = "phone number Required!")]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^[0-9]{10,12}$", ErrorMessage = "Entered phone format is not valid.")]
            public string Phone { get; set; }

            [Required(ErrorMessage = "Address line1 Required!")]
            public string AddressLine1 { get; set; }

            [Required(ErrorMessage = "Post code Required!")]
            [DataType(DataType.PostalCode)]
            [RegularExpression(@"^([A-PR-UWYZa-pr-uwyz](([0-9](([0-9]|[A-HJKSTUWa-hjkstuw])?)?)|([A-HK-Ya-hk-y][0-9]([0-9]|[ABEHMNPRVWXYabehmnprvwxy])?))[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2})", ErrorMessage = "Entered postcode format is not valid.")]
            public string PostCode { get; set; }

            [Required(ErrorMessage = "Driving license number Required!")]
            public string DrivingLicenseNumber { get; set; }
        }
       
        

    }
}
