﻿using System.ComponentModel.DataAnnotations;

namespace Elsa_Workflow.Models
{
    public class RegistrationModel
    {
        [Required]
        public string Alias { get; set; }
    }
}
