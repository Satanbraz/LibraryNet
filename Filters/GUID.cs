﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryNET.Filters
{
    public class GUID
    {
        public string GenerateTransactionId()
        {
            // Obtener la fecha y hora actual
            string dateTimePart = DateTime.Now.ToString("yyyyMMddHHmmss");

            // Obtener un GUID
            string guidPart = Guid.NewGuid().ToString().Substring(0, 8);

            // Combinar la fecha y hora con el GUID
            string customTransactionId = $"{dateTimePart}-{guidPart}";

            return customTransactionId;
        }
    }
}