﻿using System.Collections.Generic;

namespace SRLE
{
    public class IdClass
    {
        public string Name;
        public string  Id;
        public string Path;
        public int HashCode;

    }

    public class ModdedIdClass : IdClass
    {
        public string modid;
    }

    public class Category
    {
        public string CategoryName;
        public List<IdClass> Objects = new List<IdClass>();
    }
}