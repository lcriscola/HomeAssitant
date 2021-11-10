namespace AmcrestMQTT.Models
{
    public class Camera
    {
        public string Name { get; set; }
        public string UniqueId { get; set; }
        public string Host { get; set; }
        public string Password { get; set; }


        static List<Camera> _Cameras;
        public static List<Camera> GetCameras()
        {
            return _Cameras;
        }

        internal static void Set(Settings settings)
        {
            _Cameras=settings.Cameras.Select(x=> new Camera() { Host=x.Host, Password=x.Password, Name=x.Name, UniqueId=x.UniqueId }).ToList();

        }
    }
}
