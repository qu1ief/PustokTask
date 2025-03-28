namespace PustokTask.Models
{
    public class BaseEntity
    {
        public virtual int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }

}
