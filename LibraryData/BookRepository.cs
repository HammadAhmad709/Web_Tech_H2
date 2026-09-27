using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;

namespace LibraryData
{
    public class BookRepository
    {
        private string connectionString;

        public BookRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int AddBook(Book book)
        {

            string query = "INSERT INTO Books (Title, Author, Stock) VALUES (@Title, @Author, @Stock)";

            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Title", book.Title);
            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@Stock", book.Stock);

            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();

            return (i > 0 ? i : 0);
        }

        public List<Book> GetAllBooks()
        {
            List<Book> books = new List<Book>();
            string query = "SELECT * FROM Books";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Book book = new Book
                {
                    BookId = Convert.ToInt32(reader["BookId"]),
                    Title = reader["Title"].ToString(),
                    Author = reader["Author"].ToString(),
                    Stock = Convert.ToInt32(reader["Stock"])
                };
                books.Add(book);
            }
            conn.Close();
            return books;
        }

        public int UpdateBookStock(int bookId, int newStock)
        {
            string query = "Update Books SET Stock = @Stock where bookId = @BookId";

            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Stock", newStock);
            cmd.Parameters.AddWithValue("@bookId", bookId);

            int i = cmd.ExecuteNonQuery();
            conn.Close();
            return (i > 0 ? i : 0);
        }

        public int DeleteBook(int bookId)
        {
            string query = "DELETE FROM Books WHERE BookId = @BookId";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@BookId", bookId);
            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();
            return (i > 0 ? i : 0);
        }
    }
}
