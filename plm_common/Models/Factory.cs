﻿using plm_common.Models.Versioning;
using System;
using System.Collections.Generic;
using System.Text;

namespace plm_common.Models
{
    public class Factory : INeo4jNode
    {
        public string Id { get; private set; }
        public Versionable<string> name { get; private set; }

        public bool IsActive { get; private set; } = true;
    }
}