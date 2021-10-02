﻿using System.ComponentModel.DataAnnotations;

namespace Server
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ClientInfo
    {
        [Key] public string Hash { get; set; }

        public string Endpoint { get; set; }
    }
}