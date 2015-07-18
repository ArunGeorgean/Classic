namespace QuickMove.Migration.Common
{
    public class ColumnInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Size { get; set; }

        public ColumnInfo(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public ColumnInfo(string name, string type, int size) : this(name, type)
        {
            Size = size;
        }
    }
}