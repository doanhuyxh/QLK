namespace Services
{
    public class Folder
    {
        public string id { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public string _path { get; set; }
        public string type { get; set; }
        public List<Folder> children { get; internal set; }
        public List<CFile> files { get; internal set; }
    }
}
