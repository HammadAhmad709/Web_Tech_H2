using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Microsoft.Data.SqlClient;

namespace LibraryData
{
    public class LibraryService
    {
        private string connectionString;

        public LibraryService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool IssueBook(int bookId, int memberId)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            SqlTransaction trans = conn.BeginTransaction();

            string checkMemberQuery = "SELECT COUNT(*) FROM Members WHERE MemberId = @MemberId";
            SqlCommand cmdCheckMember = new SqlCommand(checkMemberQuery, conn, trans);
            cmdCheckMember.Parameters.AddWithValue("@MemberId", memberId);

            string checkStocksCmd = "SELECT stock FROM Books WHERE BookId = @BookId";
            SqlCommand cmdCheckStock = new SqlCommand(checkStocksCmd, conn, trans);
            cmdCheckStock.Parameters.AddWithValue("@BookId", bookId);

            string issueBookQuery = "INSERT INTO IssuedBooks (BookId, MemberId, IssueDate, ReturnDate) VALUES (@BookId, @MemberId, @IssueDate, NULL)";
            SqlCommand cmdIssueBook = new SqlCommand(issueBookQuery, conn, trans);
            cmdIssueBook.Parameters.AddWithValue("@BookId", bookId);
            cmdIssueBook.Parameters.AddWithValue("@MemberId", memberId);
            cmdIssueBook.Parameters.AddWithValue("@IssueDate", DateTime.Now);

            string updateStockQuery = "UPDATE Books SET Stock = Stock - 1 WHERE BookId = @bookId";
            SqlCommand cmdUpdateStock = new SqlCommand(updateStockQuery, conn, trans);
            cmdUpdateStock.Parameters.AddWithValue("@bookId", bookId);

            try
            {
                int i = (int)cmdCheckMember.ExecuteScalar();
                if (i <= 0)
                    throw new Exception(" not found");

                object stockObj = cmdCheckStock.ExecuteScalar();
                if (stockObj == null || stockObj == DBNull.Value || Convert.ToInt32(stockObj) <= 0)
                    throw new Exception("Book is out of stock or does not exist.");


                if (cmdIssueBook.ExecuteNonQuery() <= 0)
                    throw new Exception("Book not issued successfully...");

                if (cmdUpdateStock.ExecuteNonQuery() <= 0)
                    throw new Exception("Book stock not updated successfully...");

                trans.Commit();
                conn.Close();

            }

            catch (Exception ex)
            {
                trans.Rollback();
                conn.Close();
                return false;
            }

            return true;
        }

        public bool ReturnBook(int bookId, int memberId)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();


            SqlTransaction trans = conn.BeginTransaction();


            string checkIssuedBookQuery = "SELECT COUNT(*) FROM IssuedBooks WHERE BookId = @BookId AND MemberId = @MemberId AND ReturnDate IS NULL";
            SqlCommand cmdCheckIssuedBook = new SqlCommand(checkIssuedBookQuery, conn, trans);
            cmdCheckIssuedBook.Parameters.AddWithValue("@BookId", bookId);
            cmdCheckIssuedBook.Parameters.AddWithValue("@MemberId", memberId);


            string returnBookQuery = "UPDATE IssuedBooks SET ReturnDate = @ReturnDate WHERE BookId = @BookId AND MemberId = @MemberId AND ReturnDate IS NULL";
            SqlCommand cmdReturnBook = new SqlCommand(returnBookQuery, conn, trans);
            cmdReturnBook.Parameters.AddWithValue("@ReturnDate", DateTime.Now);
            cmdReturnBook.Parameters.AddWithValue("@BookId", bookId);
            cmdReturnBook.Parameters.AddWithValue("@MemberId", memberId);


            string updateStockQuery = "UPDATE Books SET Stock = Stock + 1 WHERE BookId = @bookId";
            SqlCommand cmdUpdateStock = new SqlCommand(updateStockQuery, conn, trans);
            cmdUpdateStock.Parameters.AddWithValue("@bookId", bookId);


            try
            {
                int i = (int)cmdCheckIssuedBook.ExecuteScalar();
                if (i <= 0)
                    throw new Exception("No issued record found for this book and member.");
                if (cmdReturnBook.ExecuteNonQuery() <= 0)
                    throw new Exception("Book not returned successfully...");
                if (cmdUpdateStock.ExecuteNonQuery() <= 0)
                    throw new Exception("Book stock not updated successfully...");
                
                trans.Commit();
                conn.Close();
            }

            catch (Exception ex)
            {
                trans.Rollback();
                conn.Close();
                return false;
            }

            return true;
        }
    }
}


