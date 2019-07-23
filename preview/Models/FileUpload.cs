using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace preview.Models
{
    public class FileUpload
    {
        public string Name { get; set; }

        [Required]
        public IFormFile SelectedFile { get; set; }
    }
}
