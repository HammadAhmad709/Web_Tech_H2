using System;
using System.Collections.Generic;
using LibraryData;

namespace LibraryApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;";

            BookRepository bookRepo = new BookRepository(connectionString);
            MemberRepository memberRepo = new MemberRepository(connectionString);
            LibraryService libraryService = new LibraryService(connectionString);
            BackupHelper backupHelper = new BackupHelper();

            bool running = true;

            while (running)
            {
                Console.WriteLine("\n=== LIBRARY MANAGEMENT SYSTEM ===");
                Console.WriteLine("1. Add Book");
                Console.WriteLine("2. View All Books");
                Console.WriteLine("3. Add Member");
                Console.WriteLine("4. View All Members");
                Console.WriteLine("5. Issue Book to Member (Transaction)");
                Console.WriteLine("6. Return Book (Transaction)");
                Console.WriteLine("7. Export Books to JSON");
                Console.WriteLine("8. Import Books from JSON");
                Console.WriteLine("9. Update Stock");
                Console.WriteLine("10. Delete Stock");
                Console.WriteLine("11. Exit");
                Console.Write("Enter your choice (1-11): ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                           
                        Console.Write("Enter Title: ");
                        string title = Console.ReadLine();
                        Console.Write("Enter Author: ");
                        string author = Console.ReadLine();
                        Console.Write("Enter Stock: ");
                        int stock = Convert.ToInt32(Console.ReadLine());

                        Book newBook = new Book
                        {
                            Title = title,
                            Author = author,
                            Stock = stock
                        };
                        bookRepo.AddBook(newBook);
                        Console.WriteLine("Book added successfully.");
                        break;

                    case "2":
                            
                        List<Book> books = bookRepo.GetAllBooks();
                        Console.WriteLine("--- Book List ---");
                        foreach (var b in books)
                        {
                            Console.WriteLine($"ID: {b.BookId} | Title: {b.Title} | Author: {b.Author} | Stock: {b.Stock}");
                        }
                        break;

                    case "3":
                            
                        Console.Write("Enter Member Name: ");
                        string name = Console.ReadLine();

                        Member newMember = new Member { Name = name };
                        memberRepo.AddMember(newMember);
                        Console.WriteLine("Member added successfully.");
                        break;

                    case "4":
                            
                        List<Member> members = memberRepo.GetAllMembers();
                        Console.WriteLine("--- Member List ---");
                        foreach (var m in members)
                        {
                            Console.WriteLine($"ID: {m.MemberId} | Name: {m.Name}");
                        }
                        break;

                    case "5":
                            
                        Console.Write("Enter Book ID to issue: ");
                        int issueBookId = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Enter Member ID: ");
                        int issueMemberId = Convert.ToInt32(Console.ReadLine());

                        bool issueSuccess = libraryService.IssueBook(issueBookId, issueMemberId);
                        if (issueSuccess)
                        {
                            Console.WriteLine("Book issued successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Failed to issue book (either out of stock, invalid IDs, or transaction rolled back).");
                        }
                        break;

                    case "6":
                            
                        Console.Write("Enter Book ID: ");
                        int bookId = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Enter Member ID: ");
                        int memberId = Convert.ToInt32(Console.ReadLine());

                        bool returnSuccess = libraryService.ReturnBook(bookId, memberId);
                        if (returnSuccess)
                        {
                            Console.WriteLine("Book returned successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Failed to return book (invalid credentials, already returned, or transaction rolled back).");
                        }
                        break;

                    case "7":
                            
                        Console.Write("Enter JSON file path (e.g. books_backup.json): ");
                        string exportPath = Console.ReadLine();
                        List<Book> booksToExport = bookRepo.GetAllBooks();
                        backupHelper.BackupBooksToJson(booksToExport, exportPath);
                        break;

                    case "8":
                            
                        Console.Write("Enter JSON file path to import: ");
                        string importPath = Console.ReadLine();
                        List<Book> importedBooks = backupHelper.LoadBooksFromJsonBackup(importPath);

                        Console.WriteLine("--- Imported Books (Title & Stock) ---");
                        foreach (var b in importedBooks)
                        {
                            Console.WriteLine($"Title: {b.Title} | Current Stock: {b.Stock}");
                        }
                        break;


                    case "9":
                        Console.Write("Enter Book ID to update stock: ");
                        int updateBookId = Convert.ToInt32(Console.ReadLine());

                        Console.Write("Enter New Stock quantity: ");
                        int newStock = Convert.ToInt32(Console.ReadLine());

                        bookRepo.UpdateBookStock(updateBookId, newStock);
                        Console.WriteLine("Stock updated successfully.");
                        break;

                    case "10":
                        Console.Write("Enter the Book Id to delete: ");
                        int _bookId = Convert.ToInt32(Console.ReadLine());

                        bookRepo.DeleteBook(_bookId);
                        Console.Write("Book deleted Successfully...");
                        break;
                    
                    case "11":
                            
                        running = false;
                        Console.WriteLine("Exiting application.");
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please choose between 1 and 9.");
                        break;
                }
            }
        }
    }
}
