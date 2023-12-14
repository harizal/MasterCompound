namespace MasterCompound.Models
{
    public class BaseModel<T>
    {
        public bool Error { get; set; } = false;
        public string ErrorMessage { get; set; }
        public T Data { get; set; } = default;
    }
}