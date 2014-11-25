// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntry.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core.Entities
{
    public class LogEntry : BaseEntity
    {
        public virtual DateTime Date { get; set; }

        [StringLength(255)]
        public virtual string Thread { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public virtual string Level { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(255)]
        public virtual string Logger { get; set; }

        [StringLength(255)]
        public virtual string CorrelationId { get; set; }

        [StringLength(255)]
        public virtual string LoggedUser { get; set; }

        [DataType(DataType.MultilineText)]
        public virtual string Context { get; set; }

        [DataType(DataType.MultilineText)]
        public virtual string Message { get; set; }

        [DataType(DataType.MultilineText)]
        public virtual string Exception { get; set; }
    }
}
