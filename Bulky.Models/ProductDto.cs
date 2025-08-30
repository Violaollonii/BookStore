using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    using System.ComponentModel.DataAnnotations;

    namespace BulkyBook.Models.Dto
    {
        public class ProductDto
        {
            [Required]
            public string Title { get; set; }

            [Required]
            public string ISBN { get; set; }

            [Required]
            public string Author { get; set; }

            [Range(1, 1000)]
            public double ListPrice { get; set; }

            [Range(1, 1000)]
            public double Price { get; set; }

            [Range(1, 1000)]
            public double Price50 { get; set; }

            [Range(1, 1000)]
            public double Price100 { get; set; }

            [Required]
            public int CategoryId { get; set; }
        }
    }

}
