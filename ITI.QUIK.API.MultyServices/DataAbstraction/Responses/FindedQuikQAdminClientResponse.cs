﻿using DataAbstraction.Models;

namespace DataAbstraction.Responses
{
    public class FindedQuikQAdminClientResponse
    {
        public bool IsSuccess { get; set; } = true;
        public List<string> Messages { get; set; } = new List<string>();

        public List<QuikQAdminClientModel> QuikQAdminClient { get; set; }
    }
}
