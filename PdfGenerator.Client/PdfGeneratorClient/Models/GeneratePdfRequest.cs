﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace PdfGenerator.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    public partial class GeneratePdfRequest
    {
        /// <summary>
        /// Initializes a new instance of the GeneratePdfRequest class.
        /// </summary>
        public GeneratePdfRequest() { }

        /// <summary>
        /// Initializes a new instance of the GeneratePdfRequest class.
        /// </summary>
        public GeneratePdfRequest(string documentName, byte[] html)
        {
            DocumentName = documentName;
            Html = html;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "DocumentName")]
        public string DocumentName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Html")]
        public byte[] Html { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (DocumentName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "DocumentName");
            }
            if (Html == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Html");
            }
        }
    }
}
