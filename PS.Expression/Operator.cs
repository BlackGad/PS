﻿using System;

namespace PS.Query
{
    internal abstract class Operator
    {
        #region Properties

        public string Key { get; set; }
        public Type ResultType { get; set; }

        public Type SourceType { get; set; }

        public string Token { get; set; }

        #endregion
    }
}