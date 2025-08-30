namespace BulkyBookWeb.MongoServices
{
    public class EmployeeMessageDto
    {
        public string EmployeeId { get; set; }
        public string UserName { get; set; }
        public string MessageText { get; set; }
        public bool IsChecked { get; set; } = false; // NEW FIELD
    }
}
