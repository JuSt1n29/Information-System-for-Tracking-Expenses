namespace InformationSystemForTrackingExpenses.Tables
{
    public class CategoriesTable
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; } 

        public CategoriesTable(int id, string name, string type)
        {
            this.id = id;
            this.name = name;
            this.type = type;
        }
    }
}