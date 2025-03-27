namespace PustokTask.Models
{
    public class Slider : BaseEntity
    {
        public string Title { get;set; }
        public string Description { get;set; }
        public string Image { get;set; }
        public string ButtonLink { get;set; }
        public string ButtonText { get;set; }
        public int Order { get;set; }
    }
}
