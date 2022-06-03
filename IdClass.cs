using System.Collections.Generic;

namespace SRLE
{
    public class IdClass
    {
        public string Name;
        public ulong Id;
        public string Path;

    }

    public class Category
    {
        public string CategoryName;
        public List<IdClass> Objects = new List<IdClass>();
    }
}