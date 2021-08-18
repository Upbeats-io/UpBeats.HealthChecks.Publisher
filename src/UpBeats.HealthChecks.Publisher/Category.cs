namespace UpBeats.HealthChecks.Publisher
{
    public class Category
    {
        public Category(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public string[] AsTag() => new string[] { "category:" + this.Name };

        public override string ToString()
        {
            return this.Name;
        }

        public static Category Api = new Category(nameof(Api));
        public static Category Cache = new Category(nameof(Cache));
        public static Category Database = new Category(nameof(Database));
        public static Category DocumentStore = new Category(nameof(DocumentStore));
        public static Category EventStore = new Category(nameof(EventStore));
        public static Category Ftp = new Category(nameof(Ftp));
        public static Category KeyStore = new Category(nameof(KeyStore));
        public static Category MessageQueue = new Category(nameof(MessageQueue));
        public static Category Search = new Category(nameof(Search));
        public static Category Service = new Category(nameof(Service));
        public static Category Scheduler = new Category(nameof(Scheduler));
        public static Category Test = new Category(nameof(Test));

        public static Category Placeholder = new Category(nameof(Placeholder));
        public static Category General = new Category(nameof(General));
        public static Category Unknown = new Category(nameof(Unknown));

    }
}
