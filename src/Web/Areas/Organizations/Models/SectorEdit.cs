using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Organizations.Models
{
    public class SectorEdit
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ParentId { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Name")]
        [StringLength(50)]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}