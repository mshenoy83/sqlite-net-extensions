﻿using System;

namespace SQLiteNetExtensions.Exceptions
{
    public class IncorrectRelationshipException : Exception
    {
        public string PropertyName { get; set; }
        public string TypeName { get; set; }

        public IncorrectRelationshipException(string typeName, string propertyName, string message) 
            : base($"{typeName}.{propertyName}: {message}")
        {
        }
    }
}

