using MySqlConnector;
namespace ByteToImage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string user = "root";
            string password = "";
            if (args.Length != 0)
            {
                user = args[0];
                password = args[1];
            };
            try
            {
                string strConnection = $"server=localhost;user={user};database=tc-db-main;password={password};port=3305";
                // создаём объект для подключения к БД
                MySqlConnection conn = new MySqlConnection(strConnection);
                conn.Open();
                string query = "Select hires_raster, hex(CODEKEY) from photo join personal on photo.id = personal.id";
                var input = new MySqlCommand(query, conn);
                var reader = input.ExecuteReader();
                string imagePath = Directory.GetCurrentDirectory() + "\\Images";
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }
                while (reader.Read())
                {
                    byte[] imageBytes = reader[0] as byte[];
                    MemoryStream ms = new MemoryStream(imageBytes);
                    using (var fs = new FileStream($"{imagePath}\\{reader[1]}.jpg", FileMode.Create))
                    {
                        ms.WriteTo(fs);
                    }

                }
                reader.Close();
                conn.Close();
                Console.WriteLine("Image export completed successfully");
            }
            catch (MySqlException ex)
            {
                if (ex.ToString().Contains("Access denied "))
                {
                    Console.WriteLine("Check your input credentials");
                }
                else
                {
                    Console.WriteLine("Unable to connect to DataBase server");
                }
            }

        }
    }
}