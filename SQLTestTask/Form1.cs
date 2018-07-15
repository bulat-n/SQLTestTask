using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLTestTask
{
    public partial class Form1 : Form
    {
        static SqlConnection conn;

        public Form1()
        {
            InitializeComponent();

            conn = new SqlConnection();
            conn.ConnectionString =
            "Data Source=.;" +
            "Initial Catalog=.;" +
            "Integrated Security=SSPI;";
            conn.Open();

            // TRUNCATE TABLE AutorsTable
            SqlCommand command = new SqlCommand("TRUNCATE TABLE dbo.Autors", conn);
            command.ExecuteNonQuery();

            command = new SqlCommand("TRUNCATE TABLE dbo.Books", conn);
            command.ExecuteNonQuery();

            command = new SqlCommand("TRUNCATE TABLE dbo.BooksAndAutors", conn);
            command.ExecuteNonQuery();

            AddAutor("ААА");
            AddAutor("БББ");
            AddAutor("ВВВ");
            AddAutor("ГГГ");
            AddAutor("ДДД");

            AddBook("AAA", new List<string> { "ААА", "БББ" });
            AddBook("BBB", new List<string> { "ВВВ" });
            AddBook("CCC", new List<string> { "ГГГ", "ДДД" });
            AddBook("DDD", new List<string> { "ААА" });
            AddBook("EEE", new List<string> { "БББ", "ВВВ", "ГГГ" });
            AddBook("FFF", new List<string> { "ДДД" });
            AddBook("GGG", new List<string> { "ААА", "БББ", "ВВВ", "ГГГ", "ДДД" });
            
            var commandText =
                @"select Book, AutorsNum
                from (SELECT Books.Book, count(BooksAndAutors.id) as AutorsNum
                FROM Books
                INNER JOIN BooksAndAutors ON Books.id=BooksAndAutors.BookId
                group by Books.id, Books.Book) m1
                WHERE AutorsNum >= 3";
            command = new SqlCommand(commandText, conn);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    var Book = reader.GetString(0);
                    var AutorsNum = reader.GetInt32(1);
                    dataGridView1.Rows.Add(Book, AutorsNum);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
            conn.Close();
        }

        static void AddAutor(string Autor)
        {
            var commandText = $"INSERT INTO dbo.Autors (Autor) VALUES ('{Autor}')";
            SqlCommand command = new SqlCommand(commandText, conn);
            command.ExecuteNonQuery();
        }

        static void AddBook(string BookName, List<string> BookAutors)
        {
            var commandText = $"INSERT INTO dbo.Books (Book) VALUES ('{BookName}')";
            SqlCommand command = new SqlCommand(commandText, conn);
            command.ExecuteNonQuery();
            foreach (var Autor in BookAutors)
                AddConnection(BookName, Autor);
        }

        static void AddConnection(string BookName, string BookAutor)
        {
            var commandText = $"INSERT INTO dbo.BooksAndAutors (BookId, AutorId) VALUES ((SELECT Id FROM dbo.Books WHERE Book = '{BookName}'), (SELECT Id FROM dbo.Autors WHERE Autor = '{BookAutor}'))";
            SqlCommand command = new SqlCommand(commandText, conn);
            command.ExecuteNonQuery();
        }
    }
}
