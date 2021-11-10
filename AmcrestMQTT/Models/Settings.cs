namespace AmcrestMQTT.Models
{
    public class Settings
    {
        public string MQQT_Host { get; set; }
        public string MQQT_UserName { get; set; }
        public string MQQT_Password { get; set; }
        public CameraSetting[] Cameras { get; set; }
    }

    public class CameraSetting
    {
        public string Host { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string UniqueId { get; set; }

    }

}
