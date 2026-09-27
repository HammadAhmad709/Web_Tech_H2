using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;

namespace LibraryData
{
    public class BackupHelper
    {
        public bool BackupBooksToJson(List<Book> books, string filePath)
        {
            try
            {
             string serializedJsonString = JsonSerializer.Serialize(books);
             File.WriteAllText(filePath, serializedJsonString);
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Book> LoadBooksFromJsonBackup(string filePath)
        {
            List<Book> books = new List<Book>();
            try
            {
                if(!File.Exists(filePath))
                {
                    throw new Exception("File not found");
                }

                string serializedJsonString = File.ReadAllText(filePath);
                books = JsonSerializer.Deserialize<List<Book>>(serializedJsonString);

                return books;
            }

            catch(Exception ex)
            {
                return books;
            }
        }
    }
}



