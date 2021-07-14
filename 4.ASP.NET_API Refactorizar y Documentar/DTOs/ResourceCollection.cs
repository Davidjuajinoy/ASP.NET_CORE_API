﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.DTOs
{
    public class ResourceCollection<T>
    {
        public List<T> Values{ get; set; }
        public List<Link> Links { get; set; } = new List<Link>();
        public ResourceCollection(List<T> values)
        {
            Values = values;
         
        }

    }
}
